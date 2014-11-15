using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Rikrop.Core.Data.Entities.Contracts;

namespace Rikrop.Core.Data.Entities
{
    /// <summary>
    /// Базовый класс для всех доменных объектов.
    /// </summary>
    [DataContract]
    public abstract class Entity<TId> : IEntity<TId> 
        where TId : struct
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        [DataMember]
        [Key]
        public virtual TId Id { get; set; }

        
    }
}
