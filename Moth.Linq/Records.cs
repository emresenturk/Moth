using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Moth.Linq
{
    public class Records<T> : IOrderedQueryable<T>
    {
        private readonly RecordProvider<T> provider;
        public Records()
        {
            provider = new RecordProvider<T>(this);
            Expression = Expression.Constant(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var record in provider.ExecuteEnumerator(Expression))
            {
                yield return record;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get; internal set; }
        
        public Type ElementType { get { return typeof (T); } }
        
        public IQueryProvider Provider { get { return provider; } }
    }
}