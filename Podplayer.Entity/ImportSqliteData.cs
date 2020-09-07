using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Design;
using Podplayer.Core.Models;
using Podplayer.Core.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Podplayer.Entity
{
    public class ImportSqliteData
    {

        private readonly IDesignTimeDbContextFactory<PodplayerDbContext> DbFactory;
        private readonly IDataService<Podcast> PodcastService;
        private readonly IDataService<Category> CategoryService;
        private SqliteConnection Connection;

        public ImportSqliteData(IDesignTimeDbContextFactory<PodplayerDbContext> DbFactory, IDataService<Podcast> podService, 
            IDataService<Category> catService)
        {
            this.DbFactory = DbFactory;
            PodcastService = podService;
            CategoryService = catService;
            Start();
        }

        private void Start()
        {
            CreateConnection();
            //ImportCategories();
            //ImportPodcasts();
            LinkPodcastsAndCategories();
        }

        private void ImportPodcasts()
        {
            var ctx = DbFactory.CreateDbContext(null);
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM podcasts";

           using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    var title = reader.GetString(1);
                    var author = reader.GetString(2);
                    var description = reader.GetString(3);
                    var imageLoc = reader.GetString(4);
                    var rssLoc = reader.GetString(5);

                    var pod = new Podcast
                    {
                        Title = title,
                        Description = description,
                        Author = author,
                        ImageLocation = imageLoc,
                        RssLocation = rssLoc
                    };

                    ctx.Podcasts.Add(pod);
                }
            }
            ctx.SaveChanges();
        }

        private void ImportCategories()
        {
            var ctx = DbFactory.CreateDbContext(null);
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM categories";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var title = reader.GetString(1);

                    var cat = new Category
                    {
                        Title = title
                    };

                    ctx.Categories.Add(cat);
                }
            }
            ctx.SaveChanges();
        }

        private void LinkPodcastsAndCategories()
        {
            var ctx = DbFactory.CreateDbContext(null);
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT DISTINCT pod_id, cat_id FROM podcastcategories";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var podId = reader.GetInt32(0);
                    var catId = reader.GetInt32(1);

                    var rss = GetPodcastRssUrl(podId);
                    var catName = GetCategoryName(catId);

                    var pod = ctx.Podcasts.Where(p => p.RssLocation == rss).FirstOrDefault();
                    var cat = ctx.Categories.Where(c => c.Title == catName).FirstOrDefault();

                    if (cat == null || pod == null)
                    {
                        continue;
                    }
                    try
                    {
                        ctx.PodcastCategories.Add(new PodcastCategory { CategoryId = cat.Id, PodcastId = pod.Id });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            ctx.SaveChanges();

        }

        private string GetPodcastRssUrl(int id)
        {
            string rss = null;
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT rss_loc FROM podcasts WHERE id = " + id;
            using (var r = command.ExecuteReader())
            {
                while (r.Read())
                {
                    rss = r.GetString(0);
                }
            }
            return rss;
        }

        private string GetCategoryName(int id)
        {
            string title = null;
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT title FROM categories WHERE id = " + id;
            using (var r = command.ExecuteReader())
            {
                while (r.Read())
                {
                    title = r.GetString(0);
                }
            }
            return title;
        }

        private void CreateConnection()
        {
            Connection = new SqliteConnection("Data Source=podscrape.sqlite3");
            try
            {
                Connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var hel = "asda";
            }
        }
    }
}
