using Microsoft.EntityFrameworkCore;
using Podplayer.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Podplayer.Entity.Services
{
    /// <summary>
    /// Contains extension methods for <see cref="IDataService"/>
    /// </summary>
    public static class DataServiceExtensions
    {

        public static async Task<ICollection<Podcast>> Search(this DataService<Podcast> service, string term, int skip,
            int count, bool titleOnly = false)
        {
            using var ctx = service.DbFactory.CreateDbContext(null);
            var modelSet = ctx.Podcasts;

            term = term.ToLower();

            var results = await modelSet
                .Where(pod => pod.Title.ToLower().Contains(term) 
                        || (!titleOnly && pod.Author.ToLower().Contains(term)))
                .Skip(skip).Take(count).ToListAsync();
            return results;
        }

        /// <summary>
        /// Searches for Podcasts matching a given term that are also in the given category. If none are found this will return an empty list.
        /// </summary>
        public static async Task<ICollection<Podcast>> Search(this DataService<Podcast> service, string term, string category, int skip, int count)
        {
            using var ctx = service.DbFactory.CreateDbContext(null);

            // get cat id
            var cat = await ctx.Categories.Where(c => c.Title.ToLower().Equals(category.ToLower())).FirstOrDefaultAsync();
            if (cat == null)
                return new List<Podcast>(0);
            var catId = cat.Id;

            // get list of pod ids that match term

            var podIds = await ctx.Podcasts.Where(p => p.Title.Contains(term.ToLower())).Select(p => p.Id).ToListAsync();

            var podCats = await ctx.PodcastCategories.Where(c => c.CategoryId == catId && podIds.Contains(c.PodcastId))
                .Skip(skip).Take(count).Include(c => c.Podcast).ToListAsync();

            return podCats.Select(c => c.Podcast).ToList();
        }

        public static async Task<int> NumberResultsForSearch(this DataService<Podcast> service, string term, bool titleOnly = false)
        {
            return await Task.Run(() =>
           {
                using var ctx = service.DbFactory.CreateDbContext(null);
                var modelSet = ctx.Podcasts;

                term = term.ToLower();

                var numResults = modelSet
                        .Where(pod => pod.Title.ToLower().Contains(term)
                            || (!titleOnly && pod.Author.ToLower().Contains(term)))
                        .Count();
                return numResults;
           });
            
        }

        public static async Task<int> NumberResultsForSearch(this DataService<Podcast> service, string term, string cat)
        {
            return await Task.Run(() =>
            {
                using var ctx = service.DbFactory.CreateDbContext(null);
                var modelSet = ctx.PodcastCategories;

                var category = ctx.Categories.Where(c => c.Title.ToLower().Equals(cat.ToLower())).FirstOrDefault();
                var catId = category.Id;

                term = term.ToLower();
                var numResults = modelSet
                    .Where(p => 
                    p.Podcast.Title.ToLower().Contains(term) || p.Podcast.Author.ToLower().Contains(term)
                    & p.CategoryId == catId).Count();
                return numResults;
            });

        }

        public static async Task<Podcast> SearchByRssUrl(this DataService<Podcast> service, string url)
        {
            using var ctx = service.DbFactory.CreateDbContext(null);
            var modelSet = ctx.Podcasts;

            var result = await modelSet.Where(p => p.RssLocation == url).FirstOrDefaultAsync();
            return result;
        }

        public static async Task<ICollection<Podcast>> SearchByCreator(this DataService<Podcast> service, string creator, int skip, int take)
        {
            using var ctx = service.DbFactory.CreateDbContext(null);
            var modelSet = ctx.Creators;

            creator = creator.ToLower();
            var creatorList = await modelSet.Where(c => c.Title.ToLower().Contains(creator)).Skip(skip).Take(take).Include(c => c.Podcasts)
                .ThenInclude(p => p.Podcast).ToListAsync();
            var podcasts = new List<Podcast>();
            foreach (var c in creatorList)
            {
                podcasts.AddRange(c.Podcasts.Select(c => c.Podcast));
            }

            return podcasts;
        }

        public static async Task<int> NumberPodcastsByCreator(this DataService<Podcast> service, string creator)
        {
            using var ctx = service.DbFactory.CreateDbContext(null);
            var modelSet = ctx.Podcasts;

            creator = creator.ToLower();

            var result = await modelSet.Where(p => p.Author.ToLower().Contains(creator)).CountAsync();
            return result;
        }

        /// <summary>
        /// Returns a set number of the most popular Podcasts.
        /// </summary>
        /// <param name="limit">Number of podcasts to return.</param>
        /// <returns></returns>
        public static async Task<ICollection<Podcast>> GetPopular(this DataService<Podcast> service, int limit = 10)
        {
            using var ctx = service.DbFactory.CreateDbContext(null);
            var modelSet = ctx.Podcasts;

            var results = await modelSet.OrderBy(p => p.NumberSubscriptions).Take(limit).ToListAsync();
            return results;
        }

        /// <summary>
        /// Returns a collection of the most popular Categories in the database.
        /// </summary>
        /// <param name="limit">Maximum number of Categories to return.</param>
        /// <returns></returns>
        public static async Task<ICollection<Category>> GetPopularCategories(this DataService<Podcast> service, int limit = 5)
        {
            using var ctx = service.DbFactory.CreateDbContext(null);
            var modelSet = ctx.Categories;

            var results = new List<Category>(limit);
            var titles = new List<string>{ "Comedy", "Technology", "History", "Sports", "Science"};

            foreach (var s in titles)
            {
                // Having some trouble limiting number of PodcastCategories we return when using 1 query on Categories model set 
                // with Include on Podcasts property. Doing 2 seperate queries instead.
                var cat = await modelSet.Where(c => c.Title.ToLower().Equals(s.ToLower())).FirstOrDefaultAsync();
                var podCats = await (from c in ctx.PodcastCategories select c)
                        .Where(c => c.CategoryId == cat.Id).Take(5).Include(c => c.Podcast).ToListAsync();

                cat.Podcasts = podCats;
                results.Add(cat);
            }
            
            return results;
        }

        public static async Task<ICollection<Podcast>> GetPodcastsInCategory(this DataService<Podcast> service, int categoryId,
            int skip, int count)
        {
            using var ctx = service.DbFactory.CreateDbContext(null);
            var modelSet = ctx.PodcastCategories;

            var results = await modelSet.Where(s => s.CategoryId == categoryId)
                .Skip(skip).Take(count)
                .Include(s => s.Podcast)
                .ToListAsync();

            var pods = results.Select(s => s.Podcast).ToList();
            return pods;
        }

        public static async Task<ICollection<Podcast>> GetPodcastsInCategory(this DataService<Podcast> service, string categoryName,
        int skip, int count)
        {
            using var ctx = service.DbFactory.CreateDbContext(null);
            var modelSet = ctx.PodcastCategories;

            var results = await modelSet.Where(s => s.Category.Title.ToLower().Equals(categoryName.ToLower()))
                .Skip(skip).Take(count)
                .Include(s => s.Podcast)
                .ToListAsync();

            var pods = results.Select(s => s.Podcast).ToList();
            return pods;
        }

        public static async Task<int> NumberPodcastsInCategory(this DataService<Podcast> service, string categoryName)
        {
            return await Task.Run(() =>
            {
                using var ctx = service.DbFactory.CreateDbContext(null);
                var modelSet = ctx.PodcastCategories;

                var numResults = modelSet.Where(p => p.Category.Title.ToLower().Equals(categoryName.ToLower())).Count();
                return numResults;
            });
        }
    }
}
