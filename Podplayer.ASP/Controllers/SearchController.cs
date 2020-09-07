using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Podplayer.ASP.Models;
using Podplayer.Core.Models;
using Podplayer.Core.Services.Data;
using Podplayer.Entity.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Podplayer.ASP.Controllers
{
    /// <summary>
    /// Controller used to display search results.
    /// 
    /// There are a number of Actions that will search for more specific items:
    ///     -- The default Index action will search for all podcasts with either a Title or Author that contains the given search term
    ///         'q'.
    ///     -- The Podcast action only matches the Podcast Title property.
    ///     -- The Author action only matches the Podcast Author property.
    ///     -- The Category action will search for Podcasts in the given category whose Title property matches the search term. 
    ///         If no search term is provided this will just return a list of Podcasts from that category.
    ///         
    /// All actions provide support for pagination.
    /// </summary>
    public class SearchController : BaseController
    {

        private readonly ILogger<SearchController> _logger;
        private readonly IDataService<Podcast> _dataService;

        public SearchController(ILogger<SearchController> logger, IDataService<Podcast> dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        /// <summary>
        /// Returns a paginated list of Podcasts whose Title or Author property contains the given query paramaters. If no matches 
        /// are found this will return an empty list.
        /// </remarks>
        /// <param name="q">Search term.</param>
        /// <param name="page">Which page of results to return.</param>
        /// <param name="count">How many results to return.</param>
        public async Task<IActionResult> Index(string q, int? page = 0, int? count = 20)
        {
            int startIndex = page.Value * count.Value;
            int totalItems = 0;
            string urlBase = "/Search?q=" + q;

            ICollection<Podcast> results;
            
            if (q == null || q == "")
            {
                results = new List<Podcast>(0);
            }
            else
            {
                results = await ((DataService<Podcast>)_dataService).Search(q, startIndex, count.Value);
                totalItems = await ((DataService<Podcast>)_dataService).NumberResultsForSearch(q);
            }

            int totalPages = totalItems / count.Value;

            var vm = new SearchResultViewModel
            {
               TermQuery = q,
               Models = results,
               SearchLocation = "All",
                // pagination
               TotalPages = totalPages,
               CurrentPage = page.Value,
               RequestedNumberOfItems = count.Value,
               NumberOfItems = results.Count,
               TotalNumberOfItems = totalItems,
               UrlBase = urlBase
            };
            

            if (ReturnAsPartialView())
                return PartialView(vm);

            return View(vm);
        }

        /// <summary>
        /// Returns a list of Podcasts whose title contains the given search term <paramref name="q"/>. If none match this will 
        /// return an empty list.
        /// </summary>
        /// <param name="q">Search term.</param>
        /// <param name="page">Which page of results to return.</param>
        /// <param name="count">How many results to return.</param>
        public async Task<IActionResult> Podcast(string q, int page = 0, int count = 20)
        {
            int startIndex = page * count;
            int totalItems = 0;
            string urlBase = "/search/podcast?q=" + q;

            ICollection<Podcast> results;

            if (q == null || q == "")
            {
                results = new List<Podcast>();
            }
            else
            {
                results = await ((DataService<Podcast>)_dataService).Search(q, startIndex, count, titleOnly: true);
                totalItems = await ((DataService<Podcast>)_dataService).NumberResultsForSearch(q, titleOnly: true);
            }

            int totalPages = totalItems / count;

            var vm = new SearchResultViewModel
            {
                TermQuery = q,
                Models = results,
                SearchLocation = "Podcast",
                // pagination
                TotalPages = totalPages,
                CurrentPage = page,
                RequestedNumberOfItems = count,
                NumberOfItems = results.Count,
                TotalNumberOfItems = totalItems,
                UrlBase = urlBase
            };


            if (ReturnAsPartialView())
                return PartialView("Index", vm);

            return View("Index", vm);
        }

        /// <summary>
        /// Returns a list of all Podcasts whose title contains the given search term <paramref name="q"/> and have a category 
        /// matching <param name="cat"/>. If none are found this will return an empty list.
        /// </summary>
        /// <param name="q">Optional. Searches for Podcast Title.</param>
        /// <param name="cat">Required. Used to indicate the Category title that is being searched.</param>
        public async Task<IActionResult> Category(string q, string cat, int page = 0,  int count = 20)
        {
            int startIndex = page * count;
            int totalItems = 0;
            string urlBase = "/search/category?cat=" + cat;

            ICollection<Podcast> podcasts;
            if (q == null || q == "")
            {
                if (cat != null && cat != "")
                {
                    podcasts = await ((DataService<Podcast>)_dataService).GetPodcastsInCategory(cat, startIndex, count);
                    totalItems = await ((DataService<Podcast>)_dataService).NumberPodcastsInCategory(cat);
                }
                else
                {
                    podcasts = new List<Podcast>(0);
                }
            }
            // q has value
            else
            {
                if (cat != null && cat != "")
                {
                    podcasts = await ((DataService<Podcast>)_dataService).Search(q, cat, startIndex, count);
                    totalItems = await ((DataService<Podcast>)_dataService).NumberResultsForSearch(q, cat);
                    urlBase += "&q=" + q;
                }
                else
                {
                    podcasts = new List<Podcast>(0);
                }
            }

            int totalPages = totalItems / count;

            var vm = new SearchResultViewModel
            {
                TermQuery = q,
                CategoryQuery = cat,
                Models = podcasts,
                SearchLocation = "Category",
                // pagination
                TotalPages = totalPages,
                CurrentPage = page,
                RequestedNumberOfItems = count,
                NumberOfItems = podcasts.Count,
                TotalNumberOfItems = totalItems,
                UrlBase = urlBase
            };


            if (ReturnAsPartialView())
                return PartialView(vm);

            return View(vm);

        }

        /// <summary>
        /// Returns a list of <see cref="Creator"/>s that match the given query 'q'. If no matches are found this will return an empty list.
        /// </summary>
        public async Task<IActionResult> Author(string q, int page = 0, int count = 20)
        {
            int startIndex = page * count;
            int totalItems = 0;
            string urlBase = "/search/author?q="+q;

            ICollection<Podcast> results;

            if (q == null || q == "")
            {
                results = new List<Podcast>(0);
            }
            else
            {
                results = await ((DataService<Podcast>)_dataService).SearchByCreator(q, startIndex, count);
                totalItems = await ((DataService<Podcast>)_dataService).NumberPodcastsByCreator(q);
            }

            int totalPages = totalItems / count;

            var vm = new SearchResultViewModel
            {
                TermQuery = q,
                Models = results,
                SearchLocation = "Author",
                // pagination
                TotalPages = totalPages,
                CurrentPage = page,
                RequestedNumberOfItems = count,
                NumberOfItems = results.Count,
                TotalNumberOfItems = totalItems,
                UrlBase = urlBase
            };


            if (ReturnAsPartialView())
                return PartialView("Index", vm);

            return View("Index", vm);
        }
    }
}
