using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetOption.Core.Implementations;

namespace GetOption.Core.Interface
{
    public interface IUnitOfWork
    {
        void RegisterDelete(IEntity entity, UnitOfWorkRepositoryBase entityRepository);

        void RegisterUpdate(IEntity entity, UnitOfWorkRepositoryBase entityRepository);

        void RegisterAdd(IEntity entity, UnitOfWorkRepositoryBase entityRepository);

        //void RegisterRepository(UnitOfWorkRepositoryBase repository);

        //T GetRepository<T>() where T : UnitOfWorkRepositoryBase;

         Task<bool> CommitChangesAsync();
    }

}
