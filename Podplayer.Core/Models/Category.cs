using System.Collections.Generic;

namespace Podplayer.Core.Models
{
    /// <summary>
    /// Represents a Podcast category (history, science, etc).
    /// </summary>
    public class Category : DomainObject
    {

        /// <summary>
        /// Podcasts which are part of this Category.
        /// </summary>
        public ICollection<PodcastCategory> Podcasts { get; set; }

    }
}
