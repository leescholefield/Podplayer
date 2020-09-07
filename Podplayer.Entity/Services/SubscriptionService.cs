using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Design;
using Podplayer.Core.Models;
using Podplayer.Core.Services.Data;
using Podplayer.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Podplayer.Core.Services.Rss;

namespace Podplayer.Entity.Services
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly IDataService<Podcast> _podDataService;
        private readonly IDesignTimeDbContextFactory<PodplayerDbContext> _dbContextFactory;
        private readonly IRssParser<Podcast> _rssParser;

        public SubscriptionService(
            IDataService<Podcast> podDataService,
            IDesignTimeDbContextFactory<PodplayerDbContext> dbContextFactory,
            IRssParser<Podcast> rssParser
            )
        {
            _podDataService = podDataService;
            _dbContextFactory = dbContextFactory;
            _rssParser = rssParser;
        }

        public async Task<ICollection<Podcast>> GetSubscriptions(AppUser user)
        {
            using var ctx = _dbContextFactory.CreateDbContext(null);
            var set = ctx.AppUserPodcasts;

            var query = (from m in set select m).Where(m => m.AppUserId == user.Id).Include(c => c.Podcast);

            var p = await query.ToListAsync();
            var pods = new List<Podcast>();
            foreach (var pod in p)
            {
                pod.Podcast.Subscribed = true;
                pods.Add(pod.Podcast);
            }

            return pods;
        }

        public async Task<ICollection<Podcast>> CheckSubscriptions(ICollection<Podcast> pods, AppUser user)
        {
            using var ctx = _dbContextFactory.CreateDbContext(null);
            var set = ctx.AppUserPodcasts;

            var subs = await (from m in set select m).Where(m => m.AppUserId == user.Id).ToListAsync();
            foreach (var sub in subs)
            {
                var pod = pods.Where(p => p.Id == sub.PodcastId).FirstOrDefault();
                if (pod != null)
                    pod.Subscribed = true;
            }

            return pods;
        }

        public async Task Subscribe(int podId, AppUser user)
        {
            if(PodcastExists(podId))
            {
                await SaveSubscriptionToDatabase(podId, user.Id);
            }
            else
            {
                throw new Exception("No podcast found with id " + podId);
            }

        }

        public async Task Subscribe(string podRss, AppUser user, bool saveToDbIfUnknown = true)
        {
            var pod = await SearchPodcastByRss(podRss);
            if (pod != null)
            {
                await SaveSubscriptionToDatabase(pod.Id, user.Id);
                _ = IncrementPodcastSubscriptionsCount(pod.Id);
            }
            else
            {
                if (saveToDbIfUnknown)
                {
                    pod = await _rssParser.Parse(podRss);
                    pod = await _podDataService.Save(pod);
                    await SaveSubscriptionToDatabase(pod.Id, user.Id);
                }
                else
                {
                    throw new Exception("podRss not recognized and saveToDbIfUnknown set to false");
                }
            }
        }

        public async Task<bool> IsSubscribed(int podId, AppUser user)
        {
            using var ctx = _dbContextFactory.CreateDbContext(null);
            var set = ctx.AppUserPodcasts;

            var query = (from m in set select m).Where(m => m.AppUserId == user.Id);
            query = query.Where(m => m.PodcastId == podId);
            var p = await query.FirstOrDefaultAsync();
            return p != null;
        }

        private async Task SaveSubscriptionToDatabase(int podId, string userId)
        {
            using var ctx = _dbContextFactory.CreateDbContext(null);
            ctx.AppUserPodcasts.Add(new AppUserPodcastJoin { AppUserId = userId, PodcastId = podId });
            await ctx.SaveChangesAsync();
        }

        private async Task IncrementPodcastSubscriptionsCount(int id)
        {
            using var ctx = _dbContextFactory.CreateDbContext(null);
            var pod = await ctx.Podcasts.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (pod != null)
            {
                pod.NumberSubscriptions += 1;
                await ctx.SaveChangesAsync();
            }
        }

        private bool PodcastExists(int id)
        {
            var pod = _podDataService.Get(id);
            return pod != null;
        }

        private async Task<Podcast> SearchPodcastByRss(string rss)
        {
            var pod = await ((DataService<Podcast>)_podDataService).SearchByRssUrl(rss);
            return pod;
        }

        public async Task Unsubscribe(int podId, AppUser user)
        {
            if (PodcastExists(podId))
            {
                await RemoveSubscriptionFromDatabase(podId, user.Id);
            }
            else
            {
                throw new Exception("No podcast found with id " + podId);
            }
        }

        public Task Unsubscribe(string podRss, AppUser user)
        {
            throw new NotImplementedException();
        }

        private async Task RemoveSubscriptionFromDatabase(int podId, string userId)
        {
            using var ctx = _dbContextFactory.CreateDbContext(null);
            var toRemove = ctx.AppUserPodcasts.SingleOrDefault(x => x.PodcastId == podId && x.AppUserId == userId);
            if (toRemove != null)
            {
                ctx.AppUserPodcasts.Remove(toRemove);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
