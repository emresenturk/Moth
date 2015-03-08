using System;
using System.Collections.Generic;

namespace Moth.Expressions
{
    public class ExpressionQuery : IQuery
    {
        public int QueryIndex { get; private set; }
        public ExpressionQuery()
        {
            Parameters = new List<Parameter>();
            Filters = new List<IQueryExpression>();
            Projections = new List<IQueryExpression>();
            Aggregates = new List<IQueryExpression>();
            Partitions = new List<IQueryExpression>();
            Types = new List<TypeExpression>();
            Sorts = new List<IQueryExpression>();
            QueryIndex = 0;
        }

        public IList<IQueryExpression> Filters { get; set; }
        public IList<IQueryExpression> Projections { get; set; }
        public IList<IQueryExpression> Aggregates { get; set; }
        public IList<IQueryExpression> Partitions { get; set; }
        public IList<IQueryExpression> Sorts { get; set; }
        public IQuery SubQuery { get; set; }
        public List<TypeExpression> Types { get; set; }
        public IList<Parameter> Parameters { get; set; }
        public void AddFilter(IQueryExpression expression)
        {
            AddExpression(Filters, expression);
        }

        public void AddProjection(IQueryExpression expression)
        {
            if (!(expression is TypeExpression))
            {
                AddExpression(Projections, expression);
            }
        }

        public void AddAggregation(IQueryExpression expression)
        {
            AddExpression(Aggregates, expression);
        }

        public void AddPartition(IQueryExpression expression)
        {
            AddExpression(Partitions, expression);
        }

        public void AddType(Type type)
        {
            if (Types.TrueForAll(t => t.Type != type))
            Types.Add(new TypeExpression(type));
        }

        public void AddType(TypeExpression expression)
        {
            if (Types.TrueForAll(t => t.Type != expression.Type))
            {
                Types.Add(expression);
            }
        }


        public void AddSort(IQueryExpression expression)
        {
            AddExpression(Sorts, expression);
        }

        private void AddParameters(IQueryExpression expression)
        {
            var parameterExpression = expression as ParameterExpression;
            if (parameterExpression != null)
            {
                parameterExpression.Parameter.Name = string.Format("P{0}", Parameters.Count);
                Parameters.Add(parameterExpression.Parameter);
            }
            else if (expression is BinaryExpression)
            {
                AddParameters(((BinaryExpression)expression).Left);
                AddParameters(((BinaryExpression)expression).Right);
            }
        }

        private void AddExpression(IList<IQueryExpression> expressionList, params IQueryExpression[] expressions)
        {
            foreach (var queryExpression in expressions)
            {
                expressionList.Insert(0,queryExpression);
                AddParameters(queryExpression);
            }
        }

        public void SetSubQuery(IQuery query)
        {
            SubQuery = query;
            var expressionQuery = SubQuery as ExpressionQuery;
            if (expressionQuery != null)
            {
                expressionQuery.IncrementIndex();
            }
        }

        private void IncrementIndex()
        {
            QueryIndex++;
            var subQuery = SubQuery as ExpressionQuery;
            if (subQuery != null)
            {
                subQuery.IncrementIndex();
            }
        }
    }
}