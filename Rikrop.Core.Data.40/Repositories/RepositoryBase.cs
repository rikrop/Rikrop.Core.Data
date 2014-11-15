using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using Rikrop.Core.Data.Entities.Contracts;
using Rikrop.Core.Data.Exceptions;
using Rikrop.Core.Data.Repositories.Contracts;

namespace Rikrop.Core.Data.Repositories
{
    /// <summary>
    /// Репозиторий - обертка над EF
    /// </summary>
    public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class , IEntity<TId>, IRetrievableEntity<TEntity, TId>
        where TId : struct
    {
        protected readonly IRepositoryContext RepositoryContext;
        protected readonly DbContext Context;

        protected RepositoryBase(IRepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
            Context = RepositoryContext.DbContext;
        }

        public abstract DbSet<TEntity> Data { get; }

        public TEntity Get(TId id)
        {
            return Data.Find(id);
        }

        public virtual IList<TEntity> GetAll()
        {
            return Data.ToList();
        }

        protected virtual void BeforeSave(TEntity entity, EntityState state)
        {
        }

        public virtual TEntity Save(TEntity entity)
        {
            try
            {
                var state = entity.Id.Equals(default(TId)) ? EntityState.Added : EntityState.Modified;
                Context.Entry(entity).State = state;
                BeforeSave(entity, state);
                RepositoryContext.Save();
                return entity;
            }
            catch (DbEntityValidationException e)
            {
                throw ValidationExceptionFactory.GetException(e);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null) return;
            Data.Remove(entity);
            Context.SaveChanges();
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public IList<TEntity> Get(Expression<Func<TEntity, bool>> criteria)
        {
            return Data.Where(criteria).ToList();
        }

        protected virtual RepositoryQuery<TQ> BuildQuery<TQ>()
        {
            return new RepositoryQuery<TQ>();
        }

        private static IOrderedQueryable<TVal> ApplyOrder<TVal>(IQueryable<TVal> source, string property, string methodName)
        {

            var props = property.Split('.');
            var type = typeof(TEntity);
            var arg = Expression.Parameter(type, "p");
            Expression expr = arg;
            foreach (var prop in props)
            {
                var pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TEntity), type)
                    .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<TVal>)result;
        }

        private IList<TVal> GetInternal<TVal>(IQueryable<TVal> data, RepositoryQuery<TVal> query, out int totalCount)
            where TVal : class
        {
            ApplyQuery(ref data, query, out  totalCount);
            var result = data.ToList();
            if (query.Loads.Any())
            {
                foreach (var val in result)
                {
                    foreach (var load in query.Loads)
                    {
                        Context.Entry(val).Collection(load).Load();
                    }
                }
            }
            return result;
        }

        private void ApplyQuery<TVal>(ref IQueryable<TVal> data, RepositoryQuery<TVal> query, out int totalCount)
            where TVal : class
        {
            if (query.Lookups.Any() || query.LookupsStr.Any())
            {
                foreach (var lookup in query.Lookups)
                {
                    data = data.Include(lookup);
                }
                foreach (var lookup in query.LookupsStr)
                {
                    data = data.Include(lookup);
                }
            }

            if (query.Orders.Any())
            {
                var next = false;
                foreach (var order in query.Orders)
                {
                    var orderKey = (next ? "ThenBy" : "OrderBy") + (order.IsDesc ? "Descending" : "");
                    data = ApplyOrder(data, order.Property, orderKey);
                    next = true;
                }
            }
            else
            {
                data = ApplyOrder(data, "Id", "OrderBy");
            }

            if (query.Criteria != null && query.Criteria.Any())
            {
                foreach (var expr in query.Criteria)
                    data = data.Where(expr);
            }

            totalCount = data.Count();

            if (query.SkipRows.HasValue)
                data = data.Skip(query.SkipRows.Value);

            if (query.TakeRows.HasValue)
                data = data.Take(query.TakeRows.Value);
        }

        public IList<TEntity> Get(Action<RepositoryQuery<TEntity>> queryAction, out int totalCount)
        {
            var query = BuildQuery<TEntity>();
            queryAction(query);

            return GetInternal(Data, query, out totalCount);
        }


        public IList<TEntity> Get(Action<RepositoryQuery<TEntity>> query)
        {
            int total;
            return Get(query, out total);
        }
    }
}
