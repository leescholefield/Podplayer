using Podplayer.Core.Models;
using Podplayer.Entity.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Podplayer.Entity.Services
{

    public interface ISubscriptionService
    {

        public Task<ICollection<Podcast>> GetSubscriptions(AppUser user);

        /// <summary>
        /// Goes through <paramref name="pods"/> and checks whether the user is subscribed to that Podcast. If they are then
        /// <see cref="Podcast.Subscribed"/> is set to true; if not, then false.
        /// </summary>
        /// <param name="pods">List of podcasts to check.</param>
        /// <param name="user">User to check.</param>
        /// <returns></returns>
        public Task<ICollection<Podcast>> CheckSubscriptions(ICollection<Podcast> pods, AppUser user);

        /// <summary>
        /// Returns true if the user is subsribed to this podcast.
        /// </summary>
        public Task<bool> IsSubscribed(int podId, AppUser user);

        public Task Subscribe(int podId, AppUser user);

        /// <summary>
        /// Subscribes the user to the given podcast using an RSS Url.
        /// </summary>
        /// <param name="podRss">Url of the Podcast Rss feed.</param>
        /// <param name="user">User to subscribe.</param>
        /// <param name="saveToDbIfUnknown">Whether to save to the datbase if given a podRss that is not already in the databse.</param>
        /// <exception>
        /// If the <paramref name="podRss"/> is not in the database and <paramref name="saveToDbIfUnknown"/>
        /// </exception>
        public Task Subscribe(string podRss, AppUser user, bool saveToDbIfUnknown = true);

        public Task Unsubscribe(int podId, AppUser user);

        public Task Unsubscribe(string podRss, AppUser user);
    }
}
