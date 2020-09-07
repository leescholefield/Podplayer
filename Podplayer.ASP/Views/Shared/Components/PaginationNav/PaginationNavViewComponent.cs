using Microsoft.AspNetCore.Mvc;
using Podplayer.ASP.Models;
using Podplayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Podplayer.ASP.Views.Shared.Components.PaginationNav
{
    public class PaginationNavViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IPaginatedViewModel vm)
        {
            return View();
        }
    }
}
