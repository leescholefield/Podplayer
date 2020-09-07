namespace Podplayer.Core.Models
{
    /// <summary>
    /// Represents a Join table between <see cref="Category"/> and <see cref="Podcast"/>
    /// </summary>
    public class PodcastCategory
    {

        public int PodcastId { get; set; }
        public Podcast Podcast { get; set; }

        public int CategoryId { get; set; }
        public Category Category {get; set;}
    }
}
