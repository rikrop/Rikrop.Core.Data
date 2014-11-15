namespace Rikrop.Core.Data.Entities.Contracts
{
    public interface IRetrievableEntity<TEntity, TId> : IEntity<TId>
        where TEntity : IEntity<TId>
        where TId : struct
    {
    }
}
