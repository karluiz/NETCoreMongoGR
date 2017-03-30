using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GR.Repository.Exceptions;
using GR.Repository.Mappers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GR.Repository
{
    /// <summary>
    /// Generic Mongo Repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        /// <summary>
        /// Mongo Database
        /// </summary>
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        private IMongoCollection<T> Collection { get; }

        /// <summary>
        /// Initializes the <see cref="Repository{T}"/> class.
        /// Prepare Mappings and Conventions packs
        /// </summary>
        static Repository()
        {
            MongoClassMapHelper.RegisterConventionPacks();
            MongoClassMapHelper.SetupClassMap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="System.ArgumentException">Missing MongoDB connection string</exception>
        public Repository(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentException("Missing MongoDB connection string");
            }

            var client = new MongoClient(connectionString);
            var mongoUrl = MongoUrl.Create(connectionString);
            _database = client.GetDatabase(mongoUrl.DatabaseName);
            Collection = SetupCollection();
        }

        /// <summary>
        /// Setups the collection.
        /// </summary>
        /// <returns></returns>
        private IMongoCollection<T> SetupCollection()
        {
            try
            {
                var collectionName = BuildCollectionName();
                var collection = _database.GetCollection<T>(collectionName);
                return collection;
            }
            catch (MongoException ex)
            {
                throw new CoreException(ex.Message);
            }
        }

        /// <summary>
        /// Builds the name of the collection.
        /// </summary>
        /// <returns></returns>
        private static string BuildCollectionName()
        {
            var className = typeof(T).Name;
            var pluralizedName = className.EndsWith("s") ? className : className + "s";
            pluralizedName = typeof(T).Name.EndsWith("y") ? className.Remove(className.Length - 1) + "ies" : pluralizedName;
            return pluralizedName;
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="CoreException"></exception>
        public async Task<T> Insert(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                await Collection.InsertOneAsync(entity, null, cancellationToken);
            }
            catch (MongoWriteException ex)
            {
                throw new EntityException(entity, "Insert failed because the entity already exists!", ex);
            }

            return entity;
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="CoreException">Document version conflits. (Is out of date)</exception>
        public async Task<T> Update(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            entity.UpdatedAt = DateTime.UtcNow;
            entity.Version++;

            var idFilter = Builders<T>.Filter.Eq(e => e.Id, entity.Id); //Find entity with same Id
            var result = await Collection.ReplaceOneAsync(idFilter, entity, null, cancellationToken);

            if (result != null && ((result.IsAcknowledged && result.MatchedCount == 0) || (result.IsModifiedCountAvailable && !(result.ModifiedCount > 0))))
                throw new EntityException(entity, "Entity does not exist.");

            return entity;
        }

        /// <summary>
        /// Gets entity by id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<T> Get(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Collection.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes entity by id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<T> Delete(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Collection.FindOneAndDeleteAsync(e => e.Id == id, null, cancellationToken);
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Collection.Find(e => true).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Paginations the entites.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Pagination(int top, int skip, Func<T, object> orderBy, bool ascending = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = Collection.Find(e => true).Skip(skip).Limit(top);

            if (ascending)
                return await query.SortBy(e => e.Id).ToListAsync(cancellationToken);

            return await query.SortByDescending(e => e.Id).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Search for a especific subset of entities
        /// </summary>
        /// <param name="filter">Linq filter expression.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SearchFor(FilterDefinition<T> filter)
        {
            return await Collection.Find(filter).ToListAsync();
        }
    }
}
