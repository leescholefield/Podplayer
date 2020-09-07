using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Podplayer.Entity;
using Podplayer.Entity.Identity;
using Podplayer.Entity.Services;
using System;
using System.Threading.Tasks;

namespace Podplayer.ASP.Controllers
{
    /// <summary>
    /// Controller class for retrieving a Users subscibed Podcasts.
    /// </summary>
    public class LibraryController : BaseController
    {

        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDesignTimeDbContextFactory<PodplayerDbContext> _dbContextFactory;
        private readonly ILogger<LibraryController> _logger;
        private readonly ISubscriptionService _subscriptionService;

        public LibraryController(SignInManager<AppUser> signInManager, 
            UserManager<AppUser> userManager,
            IDesignTimeDbContextFactory<PodplayerDbContext> dbContextFactory,
            ILogger<LibraryController> logger,
            ISubscriptionService subService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContextFactory = dbContextFactory;
            _logger = logger;

            _subscriptionService = subService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            var pods = await _subscriptionService.GetSubscriptions(user);

            if (ReturnAsPartialView())
                return PartialView(pods);

            return View(pods);
        }

        /// <summary>
        /// Adds a Podcast feed to the user's subscriptions. The feed can be represented by either a database id or an rss Url; whichever one 
        /// is supplied, the other must be null. If both params have a value then this will be treated as an error.
        /// 
        /// If the user is not signed in then this will redirect to the SignIn Action of the AccountController. If the user is signed in 
        /// and <paramref name="returnResult"/> is true this will return the result of <see cref="Index"/>; if returnResult is false this will 
        /// return an empty view.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rss"></param>
        /// <param name="returnResult"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Subscribe(int? id, string rss, bool returnResult = false)
        {
            // check user exists
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // validate id and rss. If method returns value they are not valid
            var validCheck = ValidateSubscribeParams(ref id, rss);
            if (validCheck != null)
            {
                return validCheck;
            }

            using var ctx = _dbContextFactory.CreateDbContext(null);
            
            if (id.HasValue)
            {
                try
                {
                    await _subscriptionService.Subscribe(id.Value, user);
                } 
                catch (Exception e)
                {
                    _logger.LogError(e, "Encountered error while trying to subscribe to podast with id {0}", id.Value);
                    return NotFound();
                }
            }
            else
            {
                try
                {
                    await _subscriptionService.Subscribe(rss, user);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Encountered error while trying to subscribe to podcast at {0}", rss);
                    return NotFound();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe(int? id, string rss, bool returnResult = false)
        {
            // check user exists
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // validate id and rss. If method returns value they are not valid
            var validCheck = ValidateSubscribeParams(ref id, rss);
            if (validCheck != null)
            {
                return validCheck;
            }

            using var ctx = _dbContextFactory.CreateDbContext(null);

            if (id.HasValue)
            {
                try
                {
                    await _subscriptionService.Unsubscribe(id.Value, user);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Encountered error while trying to unsubscribe to podast with id {0}", id.Value);
                    return NotFound();
                }
            }
            else
            {
                try
                {
                    await _subscriptionService.Unsubscribe(rss, user);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Encountered error while trying to unsubscribe to podcast at {0}", rss);
                    return NotFound();
                }
            }

            return RedirectToAction("Index");
        }

        private IActionResult ValidateSubscribeParams(ref int? id, string rss)
        {

            if (!id.HasValue & rss == null)
            {
                _logger.LogError("Calling Subscribe without suplying either an ID or url");
                return NotFound();
            }

            if (id.HasValue & rss != null)
            {
                _logger.LogError("Calling Subscribe with both an Id and url /n Ignoring id and using supplied url.");
                id = null;
            }

            return null;
        }
    }
}
