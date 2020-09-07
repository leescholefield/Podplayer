using Podplayer.Core.Models;
using Podplayer.Entity.Identity;

namespace Podplayer.Entity
{
    /// <summary>
    /// Defines a Join table between <see cref="AppUser"/> and <see cref="Podcast"/>. This would be much easier to do using Asp Framework and 
    /// EF6 (rather than Core) but that would mean re-creating this class library as a NET Framework one; we may do this in the future. 
    /// However, for now I'd rather get the app up and running and then refactor later.
    /// </summary>
    public class AppUserPodcastJoin
    {

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int PodcastId { get; set; }
        public Podcast Podcast { get; set; }

    }
}
