using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Podplayer.ASP.Views.Shared.Components.SearchBar
{
    /// <summary>
    /// ViewComponent that renders a simple search bar.
    /// </summary>
    public class SearchBarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
