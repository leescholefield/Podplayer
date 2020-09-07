using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Podplayer.Core.Models;
using Podplayer.Core.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Podplayer.Entity.Services
{
    public class DataService<T> : IDataService<T> where T : DomainObject
    {

        internal readonly IDesignTimeDbContextFactory<PodplayerDbContext> DbFactory;

        public DataService(IDesignTimeDbContextFactory<PodplayerDbContext> dbFactory)
        {
            DbFactory = dbFactory;
        }

        public int Count()
        {
            using var ctx = DbFactory.CreateDbContext(null);

            return ctx.Set<T>().Count();
        }

        public async Task<bool> Delete(T model)
        {
            return await Delete(model.Id);
        }

        public async Task<bool> Delete(int id)
        {
            using var ctx = DbFactory.CreateDbContext(null);

            var modelSet = ctx.Set<T>();

            var m = await modelSet.FirstOrDefaultAsync((e) => e.Id == id);
            if (m == null)
                return false;

            modelSet.Remove(m);
            await ctx.SaveChangesAsync();
            return true;
        }

        public async Task<T> Get(int id)
        {
            using var ctx = DbFactory.CreateDbContext(null);

            var modelSet = ctx.Set<T>();

            var model = await modelSet.FirstOrDefaultAsync(c => c.Id == id);
            return model;
        }

        public async Task<ICollection<T>> GetAll()
        {
            using var ctx = DbFactory.CreateDbContext(null);

            var modelSet = ctx.Set<T>();
            return await modelSet.ToListAsync();
        }

        public async Task<ICollection<T>> GetAll(int startIndex, int numberItemsToReturn)
        {
            using var ctx = DbFactory.CreateDbContext(null);

            var modelSet = ctx.Set<T>();
            return await modelSet.Skip(startIndex).Take(numberItemsToReturn).ToListAsync();
        }

        public async Task<T> Save(T model)
        {
            using var ctx = DbFactory.CreateDbContext(null);

            var modelSet = ctx.Set<T>();

            var result = await modelSet.AddAsync(model);
            await ctx.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<T> Update(int id, T model)
        {
            using var ctx = DbFactory.CreateDbContext(null);

            var modelSet = ctx.Set<T>();

            model.Id = id;
            modelSet.Update(model);

            await ctx.SaveChangesAsync();

            return model;
        }
    }
}
