using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Podplayer.ASP.Models;
using Podplayer.Core.Models;
using Podplayer.Core.Services.Data;
using Podplayer.Entity;
using Podplayer.Entity.Identity;
using Podplayer.Entity.Services;

namespace Podplayer.ASP.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userMananger;
        private readonly IDataService<Podcast> _dataService;
        private readonly IDataService<Category> _CatService;
        private readonly ISubscriptionService _subService;

        public HomeController(ILogger<HomeController> logger,
            IDataService<Podcast> service,
            UserManager<AppUser> userManager,
            ISubscriptionService subService,
            IDataService<Category> catService)
        {
            _logger = logger;
            _dataService = service;
            _userMananger = userManager;
            _subService = subService;
            _CatService = catService;
        }

        public async Task<IActionResult> Index(int? count = 20)
        {
            var categories = await ((DataService<Podcast>)_dataService).GetPopularCategories();

            var vm = new HomeViewModel
            {
                Models = categories
            };

            if (ReturnAsPartialView())
                return PartialView(vm);

            return View(vm);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
