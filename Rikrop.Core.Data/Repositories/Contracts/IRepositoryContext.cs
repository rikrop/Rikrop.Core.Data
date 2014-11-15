using System.Data.Entity;

namespace Rikrop.Core.Data.Repositories.Contracts
{
    public interface IRepositoryContext
    {
        DbContext DbContext { get; }

        /// <summary>
        /// Коммит данных
        /// </summary>
        void Save();
    }
}
