using System.Collections.Generic;
using Rikrop.Core.Data.Entities;
using Rikrop.Core.Data.Entities.Contracts;
using Rikrop.Core.Data.Repositories.Contracts;

namespace Rikrop.Core.Data.Repositories
{
    public abstract class DeactivatableRepositoryBase<TEntity, TId> : RepositoryBase<TEntity, TId>, IDeactivatableRepository<TEntity, TId>
        where TEntity : DeactivatableEntity<TId>, IRetrievableEntity<TEntity, TId> 
        where TId : struct 
    {
        protected DeactivatableRepositoryBase(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override IList<TEntity> GetAll()
        {
            return GetAll(false);
        }


        public IList<TEntity> GetAll(bool includeDeleted)
        {
            return includeDeleted ? base.GetAll() : Get(q => q.Filter(e => !e.IsDeleted));
        }

        public TEntity Delete(TEntity entity, bool deletePhysically)
        {
            if (!deletePhysically)
            {
                entity.IsDeleted = true;
                return Save(entity);
            }
            base.Delete(entity);
            return null;            
        }
    }
}
