using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Podplayer.Core.Services.Rss.Tests
{
    [TestClass()]
    [DeploymentItem("/TestData/revolutions.xml")]
    public class PodcastRssParserTests
    {
        private readonly PodcastRssParser Parser = new PodcastRssParser();

        private static string FILE_CONTENTS;

        [ClassInitialize()]
        public static void ClassSetup(TestContext _)
        {
            FILE_CONTENTS = File.ReadAllText("TestData/revolutions.xml");
        }

        // test I use when debugging why a particular Podcast isn't parsing correctly.
        [Ignore]
        [TestMethod()]
        public async Task RealTest()
        {
            var result = await Parser.Parse("https://www.dancarlin.com/dchh-feedburner.xml");

            Assert.AreEqual("Dan Carlin's Hardcore History", result.Title);
            Assert.AreEqual("Dan Carlin", result.Author);
            Assert.AreNotEqual(0, result.Episodes.Count);

        }

        [TestMethod()]
        public async Task Parses_Author()
        {
            var result = await Parser.ParseFromString(FILE_CONTENTS);

            Assert.AreEqual("Mike Duncan", result.Author);
        }

        [TestMethod()]
        public async Task Parses_Title()
        {
            var result = await Parser.ParseFromString(FILE_CONTENTS);

            Assert.AreEqual("Revolutions", result.Title);
        }

        [TestMethod()]
        public async Task Sets_RssLocation()
        {
            var result = await Parser.ParseFromString(FILE_CONTENTS);

            Assert.AreEqual("https://revolutionspodcast.libsyn.com/rss/", result.RssLocation);
        }

        [TestMethod()]
        public async Task Parses_ImageLocation()
        {
            var result = await Parser.ParseFromString(FILE_CONTENTS);

            Assert.AreEqual("https://ssl-static.libsyn.com/p/assets/3/4/5/f/345fbd6a253649c0/RevolutionsLogo_V2.jpg", result.ImageLocation);
        }

        [TestMethod()]
        public async Task Parses_Episodes()
        {
            var result = await Parser.ParseFromString(FILE_CONTENTS);

            Assert.AreEqual(3, result.Episodes.Count);

            var ep = result.Episodes.ToList()[0];

            Assert.AreEqual("10.39- The End of Part I", ep.Title);
            Assert.AreEqual("Mike Duncan", ep.Author);
            Assert.AreEqual("https://ssl-static.libsyn.com/p/assets/3/4/5/f/345fbd6a253649c0/RevolutionsLogo_V2.jpg", ep.ImageLocation);
            Assert.AreEqual("<p>Maybe if Nicholas hadn't been so set on rejecting the verdict of 1905, there wouldn't be a Part II to look forward to.</p>",
                ep.Description);
            Assert.AreEqual("https://traffic.libsyn.com/secure/revolutionspodcast/10.39-_The_End_of_Part_1_Master.mp3?dest-id=159998", ep.Uri.ToString());
        }
    }
}