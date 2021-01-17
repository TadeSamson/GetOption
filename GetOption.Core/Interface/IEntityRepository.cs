using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetOption.Core.Utils;

namespace GetOption.Core.Interface
{
    /// <summary>
    /// the base repository class which would allow <see cref="IUnitOfWork" persist <see cref="EntityBase"/> objects/>
    /// </summary>
    /// <remarks>The class is abstract rather than being an interface so as to abstract the persisted layer 
    /// away from the <see cref="RepositoryBase"/>e and thereby force all persists to take place through <see cref="IUnitOfWork"/>.
    /// making this an interface will force it members to be publicly accessible  </remarks>
    public interface IEntityRepository<T, TKey,TPartitionKey>
    {

        #region synchronous api


        /// <summary>
        /// Gets all entities associated with this repository.
        /// </summary>
        /// <returns>
        /// <c> <see cref="IEnumerable<T>"/> </c> where T is of type EntityBase
        /// </returns>
        List<T> GetEntities(GetOption<T> option = null);

        /// <summary>
        /// Gets an entity of this repository class.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns><c>entity T of type <see cref="EntityBase"/></c></returns>
        T GetEntity(TPartitionKey partitionKey,TKey entityId);

        /// <summary>
        /// Persists added entity.
        /// </summary>
        /// <param name="entity">The added entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        TKey PersistAddEntity(TPartitionKey partitionKey,T entity);


        /// <summary>
        /// Persists updated entity.
        /// </summary>
        /// <param name="entity">The updated entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        bool PersistUpdateEntity(TPartitionKey partitionKey,T entity);


        /// <summary>
        /// Persists deleted entity.
        /// </summary>
        /// <param name="entity">The deleted entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        bool PersistDeleteEntity(TPartitionKey partitonKey,T entity);
        #endregion
        #region asynchronous api


        /// <summary>
        /// Gets all entities associated with this repository.
        /// </summary>
        /// <returns>
        /// <c> <see cref="IEnumerable<T>"/> </c> where T is of type EntityBase
        /// </returns>
        Task<List<T>> GetEntitiesAsync(GetOption<T> option = null);


        /// <summary>
        /// Gets the entity asynchronous.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns>
        ///   <c>entity T of type <see cref="EntityBase" /></c>
        /// </returns>
        Task<T> GetEntityAsync(TPartitionKey partitionKey, TKey entityId);

        /// <summary>
        /// Persists added entity asynchronous.
        /// </summary>
        /// <param name="entity">The added entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        Task<TKey> PersistAddEntityAsync(TPartitionKey partitionKey,T entity);


        /// <summary>
        /// Persists updated entity asynchronous.
        /// </summary>
        /// <param name="entity">The updated entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        Task<bool> PersistUpdateEntityAsync(TPartitionKey partitionKey,T entity);


        /// <summary>
        /// Persists deleted entity asynchronous.
        /// </summary>
        /// <param name="entity">The deleted entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        Task<bool> PersistDeleteEntityAsync(TPartitionKey partitionKey, T entity);
        #endregion
    }
}
