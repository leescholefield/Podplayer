using Microsoft.AspNetCore.Mvc;

namespace Podplayer.ASP.Controllers
{
    public class BaseController : Controller
    {

        protected bool ReturnAsPartialView()
        {
            return Request.Headers["only-main"] == "true";
        }
    }
}
