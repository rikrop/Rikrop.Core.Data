using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Rikrop.Core.Data.Repositories
{
    public class RepositoryQuery<T>
    {
        public RepositoryQuery()
        {
            Lookups = new List<Expression<Func<T, object>>>();
            LookupsStr = new Collection<string>();
            Loads = new List<string>();
            Criteria = new List<Expression<Func<T, bool>>>();
            Orders = new List<Order>();
        }

        public int? SkipRows { get; private set; }
        public int? TakeRows { get; private set; }
        public List<Expression<Func<T, bool>>> Criteria { get; private set; }
        public ICollection<Expression<Func<T, object>>> Lookups { get; private set; }
        public ICollection<string> LookupsStr { get; private set; }
        public ICollection<string> Loads { get; private set; }
        public List<Order> Orders { get; private set; }

        public class Order
        {
            public Order(string property, bool desc)
            {
                Property = property;
                IsDesc = desc;
            }
            public string Property { get; private set; }
            public bool IsDesc { get; private set; }
        }

        public RepositoryQuery<T> Skip(int? skip)
        {
            SkipRows = skip;
            return this;
        }

        public RepositoryQuery<T> Take(int? take)
        {
            TakeRows = take;
            return this;
        }

        /// <summary>
        /// Загружает значение lookup-свойства
        /// </summary>
        /// <param name="expressions">Лямбда-выражение(я), например (user) => user.City </param>
        /// <returns></returns>
        public RepositoryQuery<T> Include(params Expression<Func<T, object>>[] expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");
            foreach (var expression in expressions)
            {
                Lookups.Add(expression);
            }

            return this;
        }

        public RepositoryQuery<T> Include(params string[] expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");
            foreach (var expression in expressions)
            {
                LookupsStr.Add(expression);
            }

            return this;
        }

        public RepositoryQuery<T> Load(params string[] navProperty)
        {
            if (navProperty == null)
                throw new ArgumentNullException("navProperty");
            foreach (var expression in navProperty)
            {
                Loads.Add(expression);
            }

            return this;
        }

        /// <summary>
        /// Накладывает фильтрацию на запрос 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public RepositoryQuery<T> Filter(Expression<Func<T, bool>> criteria)
        {
            Criteria.Add(criteria);
            return this;
        }

        /// <summary>
        /// Сортировка коллекции
        /// </summary>
        /// <param name="property">Поле для сортировки</param>
        /// <param name="desc">Направление сортировки</param>
        /// <returns></returns>
        public RepositoryQuery<T> OrderBy(string property, bool desc = false)
        {
            Orders.Add(new Order(property, desc));
            return this;
        }

        /// <summary>
        /// Сортировка коллекции
        /// </summary>
        public RepositoryQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> columnSelector)
        {
            return OrderBy(GetColumnName(columnSelector));
        }

        /// <summary>
        /// Сортировка коллекции
        /// </summary>
        public RepositoryQuery<T> OrderByDesc<TKey>(Expression<Func<T, TKey>> columnSelector)
        {
            return OrderBy(GetColumnName(columnSelector), true);
        }

        private static string GetColumnName<TKey>(Expression<Func<T, TKey>> columnSelector)
        {
            return GetPropertyName(columnSelector);
        }


        static string GetPropertyName<TSource, TProperty>(Expression<Func<TSource, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        private static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null) return true;
            if (!IsConversion(exp) || !(exp is UnaryExpression)) return false;
            memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
            return memberExp != null;
        }

        private static bool IsConversion(Expression exp)
        {
            return exp.NodeType == ExpressionType.Convert || exp.NodeType == ExpressionType.ConvertChecked;
        }
    }
}
