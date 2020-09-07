using System;
using System.Collections.Generic;
using System.Text;

namespace Podplayer.Core.Models
{
    /// <summary>
    /// Represents a Join table between <see cref="Podcast"/> and <see cref="Creator"/>
    /// </summary>
    public class PodcastCreator
    {

        public int CreatorId { get; set; }

        public Creator Creator { get; set; }

        public int PodcastId { get; set; }

        public Podcast Podcast { get; set; }

    }
}
