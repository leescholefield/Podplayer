using Podplayer.Core.Models;
using System.Collections.Generic;

namespace Podplayer.ASP.Models
{
    public class SearchResultViewModel : IPaginatedViewModel
    {
        /// <summary>
        /// Original query used to generate these results.
        /// </summary>
        public string TermQuery { get; set; }

        public string CategoryQuery { get; set; }

        /// <summary>
        /// Type of model that is being searched within. For example, "Author" or "Podacast".
        /// </summary>
        public string SearchLocation { get; set; }

        public ICollection<Podcast> Models { get; set; }
        public string UrlBase { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int NumberOfItems { get; set; }
        public int TotalNumberOfItems { get; set; }
        public int RequestedNumberOfItems { get; set; }
    }
}
