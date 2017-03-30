using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GR.Repository
{
    public interface IRepository<T> where T : IEntity
    {
        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<T> Insert(T entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the specified e                                                                         ntity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<T> Update(T entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets entity by id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<T> Get(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes by id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<T> Delete(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Paginations the entites.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> Pagination(int top, int skip, Func<T, object> orderBy, bool ascending = true, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for a especific subset of entities
        /// </summary>
        /// <param name="filter">Linq filter expression.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> SearchFor(FilterDefinition<T> filter);

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default(CancellationToken));
    }
}
