using System;

namespace Podplayer.Core.Models
{
    public class Episode : IStreamable
    {
        public Uri Uri { get; set; }

        public DateTime? PubDate { get; set; }

        public string PubDateString { 
            get
            {
                if (PubDate.HasValue)
                {
                    return PubDate.Value.ToShortDateString();
                }

                return null;
            } 
        }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public string ImageLocation { get; set; }

        /// <summary>
        /// String duration of this Episode.
        /// </summary>
        public string Duration { get; set; }
    }
}
