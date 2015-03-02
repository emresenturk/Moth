﻿using System;
using System.Collections.Generic;

namespace Moth.Expressions
{
    public class ExpressionQuery : IQuery
    {
        public ExpressionQuery()
        {
            Parameters = new List<Parameter>();
            Filters = new List<IQueryExpression>();
            Projections = new List<IQueryExpression>();
            Aggregates = new List<IQueryExpression>();
            Partitions = new List<IQueryExpression>();
            Types = new List<TypeExpression>();
            Order = new List<IQueryExpression>();
        }

        public IList<IQueryExpression> Filters { get; set; }
        public IList<IQueryExpression> Projections { get; set; }
        public IList<IQueryExpression> Aggregates { get; set; }
        public IList<IQueryExpression> Partitions { get; set; }
        public IList<IQueryExpression> Order { get; set; }
        public IQuery SubQuery { get; set; }

        public List<TypeExpression> Types { get; set; }

        public IList<Parameter> Parameters { get; set; }

        public void AddFilter(IQueryExpression expression)
        {
            AddExpression(Filters, expression);
        }

        public void AddProjection(IQueryExpression expression)
        {
            AddExpression(Projections, expression);
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
            Types.Add(new TypeExpression(type));
        }


        public void AddOrder(IQueryExpression expression)
        {
            AddExpression(Order, expression);
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
                expressionList.Add(queryExpression);
                AddParameters(queryExpression);
            }
        }
    }
}