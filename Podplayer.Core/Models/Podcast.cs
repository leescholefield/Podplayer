using System.Collections.Generic;

namespace Podplayer.Core.Models
{
    public class Podcast : DomainObject
    {

        public string ImageLocation { get; set; }

        public string Author { get; set; }

        public PodcastCreator Creator { get; set; }

        // not saved in DB
        public string Description { get; set; }

        public string RssLocation { get; set; }
        public ICollection<Episode> Episodes { get; internal set; }

        public bool Subscribed { get; set; } = false;

        /// <summary>
        /// The number of users that are subscribed to this podcast.
        /// </summary>
        public int NumberSubscriptions { get; set; }

        public ICollection<PodcastCategory> Categories { get; set; }
    }
}
