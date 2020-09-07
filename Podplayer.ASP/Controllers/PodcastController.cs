using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Podplayer.ASP.Models;
using Podplayer.Core.Models;
using Podplayer.Core.Services.Data;
using Podplayer.Core.Services.Rss;
using Podplayer.Entity.Identity;
using Podplayer.Entity.Services;
using System;
using System.Threading.Tasks;

namespace Podplayer.ASP.Controllers
{
    /// <summary>
    /// Displays information about a single <see cref="Podcast"/> including its child episodes
    /// </summary>
    public class PodcastController : BaseController
    {
        #region Dependencies
        private readonly IRssParser<Podcast> _rssParser;
        private readonly IDataService<Podcast> _dataService;
        private readonly ILogger<PodcastController> _logger;
        private readonly ISubscriptionService _subService;
        private readonly UserManager<AppUser> _userManager;

        public PodcastController(IRssParser<Podcast> rssParser, 
            IDataService<Podcast> dataService, 
            ILogger<PodcastController> logger,
            ISubscriptionService subService,
            UserManager<AppUser> userManager)
        {
            _rssParser = rssParser;
            _dataService = dataService;
            _logger = logger;
            _subService = subService;
            _userManager = userManager;
        }

        #endregion

        /// <summary>
        /// This will display a page with details of a Podcast (title, author, episodes, etc). This Podcast can be obtained via a database Id 
        /// or an Rss url. The caller must supply only one of these values, and leave the other as null; the current behavior when both are 
        /// supplied is to simply ignore the Id and use the Url - however, this may change in the future.
        /// 
        /// If no Podcast can be found, or the url cannot be parsed, this will display a generic Error page.
        /// 
        /// When calling this method with an Rss Url it will check if this address has been stored in the database. If not it will save it.
        /// </summary>
        /// <param name="id">Database Id of the podcast to parse.</param>
        /// <param name="podUrl">Url of the podcast RSS to parse.</param>
        /// <returns>
        /// A View giving details about the requested podcast. Alternativly, if the podcast cannot be parsed it will return a generic error view 
        /// located at "Views/Podcast/Error.cshtml".
        /// </returns>
        public async Task<IActionResult> Index(int? id, string podUrl)
        {
            if (!id.HasValue & podUrl == null) {
                    _logger.LogError("Calling PodcastController Index without suplying either an ID or url");
                    return NotFound();
            }

            if (id.HasValue & podUrl != null)
            {
                _logger.LogError("Calling PodcastController with both an Id and url /n Ignoring id and using supplied url.");
                id = null;
            }

            Podcast parsedPod = null;

            if (id.HasValue)
            {
                try
                {
                    parsedPod = await GetPodcastFromId(id.Value);
                    // check if user subscribed
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        // match podcasts with subscribed pods
                        var subbed = await _subService.IsSubscribed(id.Value, user);
                        parsedPod.Subscribed = subbed;
                    }

                } catch(Exception e)
                {
                    _logger.LogError(e, "Encountered error while parsing podcast with id {0}", id.Value);
                }
            }

            else
            {
                try
                {
                    var pod = await GetPodcastFromRssUrl(podUrl);
                    // not saved in database
                    if (pod == null)
                    {
                        parsedPod = await ParsePodcastFromUrl(podUrl);
                        _ = _dataService.Save(parsedPod);
                    }
                    
                    else
                    {
                        parsedPod = await _rssParser.Parse(pod);
                        // check if user subscribed
                        var user = await _userManager.GetUserAsync(User);
                        if (user != null)
                        {
                            // match podcasts with subscribed pods
                            var subbed = await _subService.IsSubscribed(id.Value, user);
                            parsedPod.Subscribed = subbed;
                        }
                    }
                } catch(Exception e)
                {
                    _logger.LogError(e, "Encountered error while parsing podcast with id {0}", id.Value);
                }
            }

            if (parsedPod == null)
                return View("Error");

            var vm = new PodcastViewModel
            {
                Podcast = parsedPod
            };

            // check headers for onlyMain
            if (ReturnAsPartialView())
            {
                return PartialView(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Returns a Podcast instance if <paramref name="rssLocation"/> exists in the database.
        /// </summary>
        private async Task<Podcast> GetPodcastFromRssUrl(string rssLocation)
        {
            // have to cast this to DataService to access extension method
            var result = await ((DataService<Podcast>)_dataService).SearchByRssUrl(rssLocation);
            return result;
        }

        private async Task<Podcast> ParsePodcastFromUrl(string url)
        {
            var podcast = await  _rssParser.Parse(url);
            if (podcast == null)
                throw new ArgumentException("Could not parse podcast at url " + url);

            return podcast;
        }

        private async Task<Podcast> GetPodcastFromId(int id)
        {
            var podcast = await _dataService.Get(id);
            if (podcast == null)
                throw new ArgumentException("No podcast could be found with the id " + id);

            return await _rssParser.Parse(podcast);
        }

    }
}
