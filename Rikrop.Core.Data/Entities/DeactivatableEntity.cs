using System.Runtime.Serialization;

namespace Rikrop.Core.Data.Entities
{
    /// <summary>
    /// Базовый класс доменных объектов, поддерживающих логическое удаление.
    /// </summary>
    public class DeactivatableEntity<TId> : Entity<TId>
        where TId : struct
    {
        [DataMember]
        public bool IsDeleted { get; set; }
    }
}
