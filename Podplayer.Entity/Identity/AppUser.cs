using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Podplayer.Entity.Identity
{
    /// <summary>
    /// Represents a user account. Along with the Properties inherited from IdentityUser this contains a custom SubscribedPodcasts column. 
    /// This is a many-to-many
    /// 
    /// Currently the only Identity fields we are concerned with are the username and password fields. This app does not send emails or 
    /// perform any two-factor authentication. This may change in the future.
    /// </summary>
    public class AppUser : IdentityUser
    {
        public ICollection<AppUserPodcastJoin> SubscribedPodcasts { get; set; } = new List<AppUserPodcastJoin>();

    }
}
