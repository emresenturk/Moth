using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Moth.Linq
{
    public class RecordProvider<TRecord> : IQueryProvider
    {
        private readonly Visitor visitor;
        private readonly Records<TRecord> records;
        private readonly string[] singleMetodNames = {"First", "FirstOrDefault", "Single", "SingleOrDefault"};
        public RecordProvider(Records<TRecord> records)
        {
            visitor = new Visitor();
            this.records = records;
        }

        internal event EventHandler<ProviderEventArgs> OnBeforeExecute;

        public IQueryable CreateQuery(Expression expression)
        {
            return CreateQuery<TRecord>(expression);
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            records.Expression = expression;
            return (IQueryable<T>) records;
        }

        public object Execute(Expression expression)
        {            
            return Execute<TRecord>(expression);
        }

        public IEnumerable<TRecord> ExecuteEnumerator(Expression expression)
        {
            if (OnBeforeExecute != null)
            {
                OnBeforeExecute(this, new ProviderEventArgs{Expression = expression});
            }
            var query = visitor.VisitAndTranslate(expression);
            query.AddType(typeof(TRecord));
            using (var executor = new ExpressionExecutor())
            {
                foreach (var record in executor.ExecuteReader<TRecord>(query))
                {
                    yield return record;
                }
            }
        } 

        public TResult Execute<TResult>(Expression expression)
        {
            var eventArgs = new ProviderEventArgs { Expression = expression };
            if (OnBeforeExecute != null)
            {
                OnBeforeExecute(this, eventArgs);
            }

            var query = visitor.VisitAndTranslate(expression);
            query.AddType(typeof(TRecord));
            using (var executor = new ExpressionExecutor())
            {
                var callExpression = expression as MethodCallExpression;
                if (callExpression == null || !singleMetodNames.Contains(callExpression.Method.Name))
                {
                    return executor.ExecuteRetrieve<TResult>(query)[0];
                }
                var result = executor.ExecuteRetrieve<TResult>(query);
                if (result.Count != 0) return result[0];
                if (callExpression.Method.Name.Contains("OrDefault"))
                {
                    return default(TResult);
                }

                throw new InvalidOperationException("No element satisfies the condition in predicate or source is empty");
            }
        }
    }

    internal class ProviderEventArgs
    {
        public Expression Expression { get; set; }
    }
}