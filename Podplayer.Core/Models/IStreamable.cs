using System;

namespace Podplayer.Core.Models
{
    /// <summary>
    /// Represents an audio source that can be Streamed.
    /// </summary>
    public interface IStreamable
    {
        /// <summary>
        /// Location of the file to be streamed.
        /// </summary>
        public Uri Uri { get; set; }
    }
}
