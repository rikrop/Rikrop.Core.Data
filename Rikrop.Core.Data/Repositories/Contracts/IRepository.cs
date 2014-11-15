using System;
using System.Collections.Generic;
using Rikrop.Core.Data.Entities.Contracts;

namespace Rikrop.Core.Data.Repositories.Contracts
{
    public interface IRepository 
    {}

    /// <summary>
    /// Репозиторий.
    /// </summary>
    public interface IRepository<TEntity, in TId> : IRepository
        where TEntity : class, IRetrievableEntity<TEntity, TId>, IEntity<TId>
        where TId : struct
    {
        /// <summary>
        /// Загрузка по ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Get(TId id);

        /// <summary>
        /// Загрузить данные по запросу
        /// </summary>
        /// <param name="query">Переметры запроса</param>
        /// <param name="totalCount">Количество записей без учета Take и Skip</param>
        IList<TEntity> Get(Action<RepositoryQuery<TEntity>> query, out int totalCount);

        /// <summary>
        /// Загрузить данные по запросу
        /// </summary>
        /// <param name="query">Переметры запроса</param>
        IList<TEntity> Get(Action<RepositoryQuery<TEntity>> query);

        /// <summary>
        /// Загрузить всю таблицу
        /// </summary>
        /// <returns></returns>
        IList<TEntity> GetAll();
        /// <summary>
        /// Сохранить запись в БД
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Save(TEntity entity);
        /// <summary>
        /// Удалить запись
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
    }
}
