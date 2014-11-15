namespace Rikrop.Core.Data.Entities.Contracts
{
    /// <summary>
    /// Интерфейс доменного объекта.
    /// </summary>
    public interface IEntity<TId>
        where TId : struct
    {
        TId Id { get; set; }
    }
}
