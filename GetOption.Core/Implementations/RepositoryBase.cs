using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GetOption.Core.Implementations;
using GetOption.Core.Interface;
using GetOption.Core.Utils;
using System.Transactions;

namespace GetOption.Core.Implementations
{
    /// <summary>
    /// <para>UnitOfWorkRepositoryBase implementation is necessary so as to be able to pass
    /// <see cref="RepositoryBase<{T},{Tkey}>"/> to <see cref="IUnitOfWork"/> without any runtime or compilation error
    /// </para>
    /// <para>If RepositoryBase were to implement <see cref="UnitOfWorkRepositoryBase"/> interfaces, passing it to <see cref="IUnitOfWork"/>
    /// results into an exception because RepositoryBase is a generic and should be a generic base class while the repository instance required
    /// by IUnitOfWork is non Generic. this non generic repository required by <see cref="IUnitOfWork"/> ensure that IUnitOfWork can work
    /// with each object's Repository given to it to work on for (update delete insert) without knowing their type. 
    /// This advantage ensure that a single non generic type of IUnitOfWork is required to monitor all entities.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The repository item type which must be of type <see cref="IEntity"/></typeparam>
    /// 
    public abstract class RepositoryBase<T> : UnitOfWorkRepositoryBase, IEnlistmentNotification where T : class, IEntity
    {

        Boolean enlisted;
        Dictionary<IEntity, EnlistmentOperations> transactionElements = new Dictionary<IEntity, EnlistmentOperations>();

        private IUnitOfWork unitOfWork;
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{T}"/> class.
        /// </summary>
        /// <param name="_unitOfWork">The _unit of work that will be associated with this repository.</param>
        /// <exception cref="System.Exception">IUnitOfWork instance is require for class initialization</exception>
        public RepositoryBase(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork ?? throw new Exception("IUnitOfWork instance is require for class initialization");
        }


        /// <summary>
        /// Pushes an entity of type <see cref="IEntity"/> into <see cref="IUnitOfWork"/> for deletion after call to it CommitChanges method.
        /// </summary>
        /// <param name="entity">The entity to Add.</param>
        public void QueueForRemove(T entity)
        {
            this.unitOfWork.RegisterDelete(entity, this);
        }


        /// <summary>
        /// Pushes an entity of type <see cref="IEntity"/> into <see cref="IUnitOfWork"/> for update after call to it <c> CommitChanges()</c> method.
        /// </summary>
        /// <param name="entity">The entity to Update.</param>
        public void QueueForUpdate(T entity)
        {
            this.unitOfWork.RegisterUpdate(entity, this);
        }


        /// <summary>
        /// Pushes an entity of type <see cref="IEntity"/> into a <see cref="IUnitOfWork"/>  for persistence after call to it <c> CommitChanges()</c> method.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        public void QueueForAdd(T entity)
        {
            this.unitOfWork.RegisterAdd(entity, this);
        }



        #region overriding asynchronous methods signature from UnitOfWorkRepositoryBase responsible for ACID (Atomic-Constistency-Isolation-Durability) operation


        /// <summary>
        /// Prepares an entity to take part in a unitOfWork add operation.
        /// It does nothing than call virtual TransactionalAddAsync that knows how to perform an ACID Add
        /// /// Will be called by UnitofWork implementation
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>
        ///   <c>True</c> 
        internal override Task<CrudResult<String>> UnitOfWorkAddAsync(IEntity entity)
        {
            return this.TransactionalAddAsync(entity);
        }


        /// <summary>
        /// Prepares an entity to take part in a unitOfWork update operation. 
        /// Will be called by UnitofWork implementation
        /// It does nothing than call virtual TransactionalUpdateAsync that knows how to perform an ACID update
        /// </summary>
        /// <param name="entity">The updated entity to persist</param>
        /// <returns>
        ///   <c>True</c> if prepared, <c>False</c> otherwise
        /// </returns>
        internal override Task<CrudResult<bool>> UnitOfWorkUpdateAsync(IEntity entity)
        {
            return this.TransactionalUpdateAsync(entity);
        }




        /// <summary>
        /// Prepares an entity to take part in a unitOfWork delete operation
        /// /// Will be called by UnitofWork implementation
        /// It does nothing than call virtual TransactionalDeleteAsync that knows how to perform an ACID delete
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <returns>
        ///   <c>True</c> 
        internal override Task<CrudResult<bool>> UnitOfWorkDeleteAsync(IEntity entity)
        {
            return this.TransactionalDeleteAsync(entity);
        }


        #endregion

        #region asynchronous api

        /// <summary>
        /// Gets all entities associated with this repository.
        /// </summary>
        /// <returns>
        /// <c> <see cref="IEnumerable<T>"/> </c> where T is of type EntityBase
        /// </returns>
        public abstract Task<List<T>> GetEntitiesAsync(GetOption<T> option = null);


        /// <summary>
        /// Gets the entity asynchronous.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns>
        ///   <c>entity T of type <see cref="EntityBase" /></c>
        /// </returns>
        /// 
        public abstract Task<T> GetEntityAsync(string entityId, GetOption<T> option = null);

        /// <summary>
        /// Persists added entity asynchronous.
        /// </summary>
        /// <param name="entity">The added entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        protected abstract Task<CrudResult<string>> PersistAddEntityAsync(T entity);


        /// <summary>
        /// Persists updated entity asynchronous.
        /// </summary>
        /// <param name="entity">The updated entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        protected abstract Task<CrudResult<bool>> PersistUpdateEntityAsync(T entity);


        /// <summary>
        /// Persists deleted entity asynchronous.
        /// </summary>
        /// <param name="entity">The deleted entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        protected abstract Task<CrudResult<bool>> PersistDeleteEntityAsync(T entity);
        #endregion




        #region TransactionManager Implementation


        void enlistForTransaction()
        {
            if (this.enlisted == false)
            {
                Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);
                this.enlisted = true;
            }
        }

        public void Commit(Enlistment enlistment)
        {
            enlistment.Done();
        }
        public void InDoubt(Enlistment enlistment)
        {
            Rollback(enlistment);
            //enlistment.Done();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }



        public void Rollback(Enlistment enlistment)
        {
            foreach (IEntity entity in transactionElements.Keys)
            {
                if (entity == null) //doing this for debugging incase I forgot to remove it later. 
                {

                }
                EnlistmentOperations operation = transactionElements[entity];
                switch (operation)
                {
                    case EnlistmentOperations.Add: PersistDeleteEntityAsync(entity as T); break;
                    case EnlistmentOperations.Delete: PersistAddEntityAsync(entity as T); break;
                    case EnlistmentOperations.Update: PersistUpdateEntityAsync(entity as T); break;
                }
            }
            enlistment.Done();
        }

        #endregion


        #region Methods in charge of Transaction operations that can rollback. Can be overriden to behave differently by derive classes

        /// <summary>
        /// Performs Add operation that can be rollback
        /// </summary>
        /// <param name="entity"></param>
        /// <returns><c>True</c> if operation success, <c>False</c> otherwise</returns>
        protected async virtual Task<CrudResult<string>> TransactionalAddAsync(IEntity entity)
        {
            if (Transaction.Current != null)
            {
                enlistForTransaction();
                transactionElements.Add(entity, EnlistmentOperations.Add);
                return await this.PersistAddEntityAsync(entity as T);
            }
            else
                return await this.PersistAddEntityAsync(entity as T);
        }


        /// <summary>
        /// Performs Update operation that can be rollback
        /// </summary>
        /// <param name="entity"></param>
        /// <returns><c>True</c> if operation success, <c>False</c> otherwise</returns>
        protected async virtual Task<CrudResult<bool>> TransactionalUpdateAsync(IEntity entity)
        {
            if (Transaction.Current != null)
            {
                enlistForTransaction();
                T original = await GetEntityAsync(entity.Id);

                if (original == null)
                    return CrudResult<bool>.Success(true);
                transactionElements.Add(original, EnlistmentOperations.Update);

                return await this.PersistUpdateEntityAsync(entity as T);
            }
            else
                return await this.PersistUpdateEntityAsync(entity as T);
        }


        /// <summary>
        /// Performs Delete operation that can be rollback
        /// </summary>
        /// <param name="entity"></param>
        /// <returns><c>True</c> if operation success, <c>False</c> otherwise</returns>
        protected async virtual Task<CrudResult<bool>> TransactionalDeleteAsync(IEntity entity)
        {
            if (Transaction.Current != null)
            {
                enlistForTransaction();
                T original = await GetEntityAsync(entity.Id);
                if (original == null)
                    return CrudResult<bool>.Success(true);
                transactionElements.Add(original, EnlistmentOperations.Delete);
                return await PersistDeleteEntityAsync(entity as T);
            }
            else
                return await PersistDeleteEntityAsync(entity as T);
        }

        #endregion

    }
}
