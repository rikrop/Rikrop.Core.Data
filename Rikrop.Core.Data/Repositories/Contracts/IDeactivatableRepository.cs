using System.Collections.Generic;
using Rikrop.Core.Data.Entities;
using Rikrop.Core.Data.Entities.Contracts;

namespace Rikrop.Core.Data.Repositories.Contracts
{
    public interface IDeactivatableRepository<TEntity, in TId> : IRepository<TEntity, TId>
        where TEntity : DeactivatableEntity<TId>, IRetrievableEntity<TEntity, TId>
        where TId : struct
    {
        IList<TEntity> GetAll(bool includeDeleted);
        TEntity Delete(TEntity entity, bool deletePhysically);
    }
}
