using Podplayer.Core.Models;
using System.Collections.Generic;

namespace Podplayer.ASP.Models
{
    /// <summary>
    /// Interface for a Model that can be Paginated.
    /// </summary>
    public interface IPaginatedViewModel
    {

        /// <summary>
        /// Url of the page that has been requested. This will then have 'page' and 'count' appended to it on further requests.
        /// </summary>
        public string UrlBase { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int NumberOfItems { get; set; }

        public int TotalNumberOfItems { get; set; }

        /// <summary>
        /// The number of items that was requested by the caller.
        /// </summary>
        public int RequestedNumberOfItems { get; set; }
    }
}
