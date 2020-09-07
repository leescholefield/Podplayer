using System.Collections.Generic;

namespace Podplayer.Core.Models
{
    public class Creator : DomainObject
    {

        public ICollection<PodcastCreator> Podcasts { get; set; }
    }
}
