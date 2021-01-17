using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using GetOption.Core.Interface;

namespace GetOption.Core.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {

        List<KeyValuePair<IEntity, UnitOfWorkRepositoryBase>> deletedEntities = new List<KeyValuePair<IEntity, UnitOfWorkRepositoryBase>>();
        List<KeyValuePair<IEntity, UnitOfWorkRepositoryBase>> updatedEntities = new List<KeyValuePair<IEntity, UnitOfWorkRepositoryBase>>();
        List<KeyValuePair<IEntity, UnitOfWorkRepositoryBase>> addedEntities = new List<KeyValuePair<IEntity, UnitOfWorkRepositoryBase>>();


        // commenting out because this is resulting in modifying unitOfWork entities while being added resulting in foreach modified element error
        //private static UnitOfWork instance = null;
        //public static UnitOfWork Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //            instance = new UnitOfWork();
        //        return instance;
        //    }
        //}

        //private UnitOfWork()
        //{

        //}

        public void RegisterDelete(IEntity entity, UnitOfWorkRepositoryBase entityRepository)
        {
            this.deletedEntities.Add(new KeyValuePair<IEntity, UnitOfWorkRepositoryBase>( entity, entityRepository));
        }


        public void RegisterUpdate(IEntity entity, UnitOfWorkRepositoryBase entityRepository)
        {

            this.updatedEntities.Add(new KeyValuePair<IEntity, UnitOfWorkRepositoryBase>(entity, entityRepository));
        }

        public void RegisterAdd(IEntity entity, UnitOfWorkRepositoryBase entityRepository)
        {
            this.addedEntities.Add(new KeyValuePair<IEntity, UnitOfWorkRepositoryBase>(entity, entityRepository));
        }


        public Task<bool> CommitChangesAsync()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            List<System.Threading.Tasks.Task> taskList = new List<System.Threading.Tasks.Task>();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (KeyValuePair<IEntity, UnitOfWorkRepositoryBase> kvp in this.deletedEntities)
                    {
                        if(kvp.Key!=null && kvp.Value!=null)
                        taskList.Add(kvp.Value.UnitOfWorkDeleteAsync(kvp.Key));
                    }
                    foreach (KeyValuePair<IEntity, UnitOfWorkRepositoryBase> kvp in this.updatedEntities)
                    {
                        if (kvp.Key != null && kvp.Value != null)
                            taskList.Add(kvp.Value.UnitOfWorkUpdateAsync(kvp.Key));
                    }
                    foreach (KeyValuePair<IEntity, UnitOfWorkRepositoryBase> kvp in this.addedEntities)
                    {
                        if (kvp.Key != null && kvp.Value != null)
                            taskList.Add(kvp.Value.UnitOfWorkAddAsync(kvp.Key));
                    }
                    System.Threading.Tasks.Task.WaitAll(taskList.ToArray());
                    scope.Complete();
                    this.deletedEntities.Clear();
                    this.addedEntities.Clear();
                    this.updatedEntities.Clear();
                    tcs.SetResult(true);

                }
                catch (Exception ex)
                {
                    this.deletedEntities.Clear();
                    this.addedEntities.Clear();
                    this.updatedEntities.Clear();
                    tcs.SetException(ex);
                }

            }
            return tcs.Task;

        }


    }
}
