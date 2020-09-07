using Podplayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Podplayer.Core.Services.Rss
{
    /// <summary>
    /// Implementation of <see cref="IRssParser{T}"/> that parses an RSS feed into an instance of <see cref="Podcast"/>.
    /// </summary>
    public class PodcastRssParser : IRssParser<Podcast>
    {

        private static readonly XNamespace _itunesNamespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        private static readonly XNamespace _atomNamespace = "http://www.w3.org/2005/Atom";

        public async Task<Podcast> Parse(string url)
        {
            var client = new WebClient();
            string contents;
            try
            {
                contents = await client.DownloadStringTaskAsync(url);
            }
            catch (WebException e)
            {
                throw new Exception("Could not download url contents", e);
            }

            return await ParseFromString(contents);

        }

        public async Task<Podcast> Parse(Podcast item)
        {
            var result = await Parse(item.RssLocation);
            result.Id = item.Id;
            return result;
        }

        public async Task<Podcast> ParseFromString(string content)
        {
            XDocument feedXML = XDocument.Parse(content);

            var channelRoot = feedXML.Root.Element("channel");

            var author = await GetAuthor(channelRoot);
            var title = GetTitle(channelRoot);

            var imageLoc = TryGetElementAtrtibuteValue(channelRoot, "image", "url", defaultVal: null);
            if (imageLoc == null)
                imageLoc = TryGetElementAtrtibuteValue(channelRoot, _itunesNamespace + "image", "href", defaultVal: null);

            var rssLoc = GetRssLocation(channelRoot);
            var episodes = ParseEpisodes(channelRoot, author);
            var description = GetDescription(channelRoot);

            return new Podcast
            {
                Author = author,
                Title = await title,
                RssLocation = await rssLoc,
                ImageLocation = imageLoc,
                Episodes = await episodes,
                Description = await description
            };

        }

        /// <summary>
        /// TODO need to handle malformed episodes better. Try and get an element, if cant set null. If any essential elemements are null 
        /// skip to next episode
        /// </summary>
        /// <param name="root"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        private Task<List<Episode>> ParseEpisodes(XElement root, string author)
        {
            return Task.Run(() =>
           {
               var eps = new List<Episode>();
               foreach (var ep in root.Elements("item"))
               {
                   try
                   {
                       var title = TryGetElementValue(ep, "title", throwIfCantFind: true);
                       var description = TryGetElementValue(ep, "description", defaultVal: null);
                       var url = TryGetElementAtrtibuteValue(ep, "enclosure", "url", throwIfCantFind: true);
                       var imageLoc = TryGetElementAtrtibuteValue(ep, _itunesNamespace + "image", "href", defaultVal: null);
                       var pubDateString = TryGetElementValue(ep, "pubDate", defaultVal: null);
                       var pubDateDt = TryParseDateTime(pubDateString);
                       // the 'itunes:duration' tag is supposed to be in format 'HH:MM:SS', however I have found many feeds that don't follow 
                       // this. If a large percentage don't follow this I'll have to add some parsing.
                       var durationString = TryGetElementValue(ep, _itunesNamespace + "duration", "NA");

                       eps.Add(new Episode
                       {
                           Title = title,
                           Author = author,
                           Description = description,
                           ImageLocation = imageLoc,
                           Uri = new Uri(url),
                           PubDate = pubDateDt,
                           Duration = durationString
                       });
                   } 
                   catch (Exception e)
                   {
                      // ignore
                   }
               }
               return eps;
           });
        }

        private string TryGetElementValue(XElement root, XName elemName, string defaultVal = "Unknown", bool throwIfCantFind = false)
        {
            string val = null;
            var elem = root.Element(elemName);
            if (elem != null)
                val = elem.Value;

            if (val == null)
            {
                if (throwIfCantFind)
                {
                    throw new ArgumentException("Cannot find element with name " + elemName);
                }
                val = defaultVal;
            }

            return val;
        }

        /// <summary>
        /// Tries to get the string Value of an Attribute. If the attribute cannot be found this will either return the provided defaultVal or 
        /// if <paramref name="throwIfCantFind"/> is true, throw an ArgumentException.
        /// </summary>
        /// <example>
        ///     <code>
        ///         string imageUrl = TryGetElementAttributeValue(root, "image", "href", throwIfCantFind: true);
        ///     </code>
        /// This will attempt to find the value of the "href" Attribute of an "image" Element that is a descendant of root. If it cannot find 
        /// it an exception will be thrown.
        /// </example>
        /// <param name="root">Root Element to search.</param>
        /// <param name="elemName">Name of the attribute that contains the Attribute.</param>
        /// <param name="attribName">Name of the Attribute to search for.</param>
        /// <param name="defaultVal">Optional default value to return if the attribute cannot be found and throwIfCantFind is false.</param>
        /// <param name="throwIfCantFind">Whether to throw an Exception if the atrribute cannot be found. </param>
        /// <returns></returns>
        private string TryGetElementAtrtibuteValue(XElement root, XName elemName, XName attribName, 
            string defaultVal = "Unknown", bool throwIfCantFind = false)
        {
            string val = null;
            var elem = root.Element(elemName);
            if (elem != null)
            {
                var attrib = elem.Attribute(attribName);
                if (attrib != null)
                    val = attrib.Value;
            }

            if (val == null)
            {
                if (throwIfCantFind)
                {
                    throw new ArgumentException("Cannot find element with name " + elemName);
                }
                val = defaultVal;
            }

            return val;
        }

        private DateTime? TryParseDateTime(string dtString)
        {
            if (dtString == null)
                return null;

            try
            {
                return DateTime.Parse(dtString);
            } 
            catch(Exception)
            {
                return null;
            }
        }


        #region Podcast Properties

        private Task<string> GetAuthor(XElement root, string defaultVal = "Unknown Author")
        {
            return Task.Run(() => 
            {
                var elem = root.Element(_itunesNamespace + "author");
                if (elem != null) return elem.Value;
                return defaultVal;
            });

        }

        private Task<string> GetDescription(XElement root)
        {
            return Task.Run(() =>
           {
               var elem = root.Element("description");
               if (elem != null) return elem.Value;
               return null;
           });
        }

        private Task<string> GetTitle(XElement root, string defaultVal = "Unknown Title")
        {
            return Task.Run(() =>
            {
                var elem = root.Element("title");
                if (elem != null) return elem.Value;
                return defaultVal;
            });
        }

        private Task<string> GetRssLocation(XElement channelRoot)
        {
            return Task.Run(() =>
            {
                var linkElem = channelRoot.Element(_atomNamespace + "link");
                if (linkElem == null)
                    return null;
                var elem = linkElem.Attribute("href");
                if (elem != null) return elem.Value;
                return null;
            });
        }

        #endregion
    }
}
