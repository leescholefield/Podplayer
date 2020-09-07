using Podplayer.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Podplayer.Core.Services.Data
{
    public interface IDataService<T> where T : DomainObject
    {

        /// <summary>
        /// Returns every instance of <typeparamref name="T"/> in the Database.
        /// </summary>
        Task<ICollection<T>> GetAll();

        /// <summary>
        /// Returns a limited number of items starting at <paramref name="startIndex"/>.
        /// </summary>
        Task<ICollection<T>> GetAll(int startIndex, int numberItems);

        /// <summary>
        /// Returns a single object of type <typeparamref name="T"/> matching the given id. If no record exists with the given id this 
        /// will return null.
        /// </summary>
        Task<T> Get(int id);

        /// <summary>
        /// Saves <paramref name="model"/> to the database and then returns the saved object.
        /// </summary>
        Task<T> Save(T model);

        /// <summary>
        /// Ovewrites the values of <paramref name="model"/> to the database record matching <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the model to update.</param>
        /// <param name="model">New Model that will be saved.</param>
        /// <returns>The new Model that was saved.</returns>
        Task<T> Update(int id, T model);

        /// <summary>
        /// Deletes the given Model from the database.
        /// </summary>
        /// <param name="model">Model to delete.</param>
        Task<bool> Delete(T model);

        /// <summary>
        /// Deletes the model matching the given id from the database.
        /// </summary>
        /// <param name="id">Id of the record to delete.</param>
        Task<bool> Delete(int id);

        /// <summary>
        /// Returns the number of items of type <typeparamref name="T"/> in the database.
        /// </summary>
        int Count();
    }
}
