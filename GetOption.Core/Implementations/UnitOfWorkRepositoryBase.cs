using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetOption.Core.Interface;
using GetOption.Core.Utils;
using System.Transactions;

namespace GetOption.Core.Implementations
{
    /// <summary>
    /// the base repository class which would allow <see cref="IUnitOfWork" persist <see cref="EntityBase"/> objects/>
    /// </summary>
    /// <remarks>The class is abstract rather than being an interface so as to abstract the persisted layer 
    /// away from the <see cref="RepositoryBase"/> and thereby force all persists to take place through <see cref="IUnitOfWork"/>. 
    /// making this an interface will force it members to be publicly accessible  </remarks>
    public abstract class UnitOfWorkRepositoryBase
    {

     

        #region asynchronous api


        /// <summary>
        /// Persists added entity asynchronous.
        /// </summary>
        /// <param name="entity">The added entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        internal abstract Task<CrudResult<String>> UnitOfWorkAddAsync(IEntity entity);


        /// <summary>
        /// Persists updated entity asynchronous.
        /// </summary>
        /// <param name="entity">The updated entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        internal abstract Task<CrudResult<bool>> UnitOfWorkUpdateAsync(IEntity entity);


        /// <summary>
        /// Persists deleted entity asynchronous.
        /// </summary>
        /// <param name="entity">The deleted entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        internal abstract Task<CrudResult<bool>> UnitOfWorkDeleteAsync(IEntity entity);


        #endregion


     




    }



    

}