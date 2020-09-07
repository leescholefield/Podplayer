using Podplayer.Core.Models;
using System.Collections.Generic;

namespace Podplayer.ASP.Models
{
    /// <summary>
    /// ViewModel for the site HomePage.
    /// 
    /// This will display a paginated list of popular categories.
    /// </summary>
    public class HomeViewModel
    {

        public ICollection<Category> Models { get; set; }
    }
}
