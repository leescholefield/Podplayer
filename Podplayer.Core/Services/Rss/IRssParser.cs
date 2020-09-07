using System.Threading.Tasks;

namespace Podplayer.Core.Services.Rss
{
    /// <summary>
    /// Responsible for parsing RSS feeds into instances of <typeparamref name="T"/>
    /// </summary>
    public interface IRssParser<T>
    {
        /// <summary>
        /// Parses the given url and returns an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="url">Location of the RSS Feed to parse.</param>
        /// <returns>An instance of type T representing the parsed feed. Returns null if the feed could not be parsed.</returns>
        Task<T> Parse(string url);

        Task<T> Parse(T item);

        Task<T> ParseFromString(string content);

    }
}
