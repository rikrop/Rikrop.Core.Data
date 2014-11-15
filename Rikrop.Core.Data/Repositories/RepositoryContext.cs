using System.Data.Entity;
using Rikrop.Core.Data.Repositories.Contracts;

namespace Rikrop.Core.Data.Repositories
{
    /// <summary>
    /// Обёртка над контекстом работы с хранилищем данных.
    /// </summary>
    public class RepositoryContext : IRepositoryContext
    {
        private readonly DbContext _dbContext;

        public RepositoryContext(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // TODO: VM: Здесь должна лежать абстракция, которая отдаёт наборы данных.
        public DbContext DbContext { get { return _dbContext; } }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
