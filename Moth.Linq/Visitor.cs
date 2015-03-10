using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moth.Expressions;
using BinaryExpression = System.Linq.Expressions.BinaryExpression;
using ConstantExpression = System.Linq.Expressions.ConstantExpression;
using MemberExpression = System.Linq.Expressions.MemberExpression;

namespace Moth.Linq
{
    internal class Visitor : ExpressionVisitor
    {
        private static readonly string[] QueryMethodNames =
        {
            "Where", 
            "Select", 
            "OrderBy", 
            "OrderByDescending", 
            "ThenBy",
            "ThenByDescending",
            "First",
            "FirstOrDefault",
            "Single",
            "SingleOrDefault",
            "Last",
            "LastOrDefault",
            "Average",
            "Count",
            "Max",
            "Min",
            "Sum"
        };

        private static readonly string[] FilterMethodNames =
        {
            "First",
            "FirstOrDefault",
            "Last",
            "LastOrDefault",
            "Single",
            "SingleOrDefault",
            "Where"
        };

        private static readonly string[] AggregateMethodNames =
        {
            "Average",
            "Count",
            "Max",
            "Min",
            "Sum"
        };

        private static readonly string[] ProjectionMethodNames =
        {
            "Average",
            "Count",
            "Max",
            "Min",
            "Select",
            "Sum"
        };

        private static readonly string[] PartitionMethodNames =
        {
            "First",
            "FirstOrDefault",
            "Last",
            "LastOrDefault",            
            "Single",
            "SingleOrDefault",
            "Skip",
            "SkipWhile",
            "Take",
            "TakeWhile"
        };

        private static readonly string[] OrderMethodNames =
        {
            "OrderBy",
            "OrderByDescending",
            "ThenBy",
            "ThenByDescending"
        };

        private ExpressionQuery query;
        private ExpressionQuery queryToReturn;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            if (QueryMethodNames.Contains(node.Method.Name))
            {
                if (FilterMethodNames.Contains(method.Name) && node.Arguments.Count > 1)
                {
                    query.AddFilter(Translator.TranslateExpression(Visit(node.Arguments[1])));
                }

                if (ProjectionMethodNames.Contains(method.Name) && node.Arguments.Count > 1)
                {
                    var projectionExpression = Translator.TranslateExpression(Visit(node.Arguments[1]));
                    if (method.Name == "Select" && projectionExpression is TypeExpression)
                    {
                        query.AddType(projectionExpression as TypeExpression);
                    }
                    else
                    {
                        var lastSubQuery = LastQuery(queryToReturn);
                        query = new ExpressionQuery();
                        lastSubQuery.SubQuery = query;
                        query.AddProjection(projectionExpression);   
                    }
                }

                if (AggregateMethodNames.Contains(method.Name))
                {
                    var expressionToVisit = node.Arguments.Count < 2
                        ? node.Arguments[0]
                        : node.Arguments[1];
                    MethodType methodType;
                    if (Enum.TryParse(method.Name, out methodType))
                    {
                        query.AddAggregation(new MethodExpression(methodType,
                            Translator.TranslateExpression(Visit(expressionToVisit))));
                    }
                    else
                    {
                        query.AddAggregation(Translator.TranslateExpression(Visit(expressionToVisit)));
                    }
                }

                if (PartitionMethodNames.Contains(method.Name))
                {
                    MethodType methodType;
                    if (Enum.TryParse(method.Name, out methodType))
                    {
                        query.AddPartition(new MethodExpression(methodType, Translator.TranslateExpression(Visit(node.Arguments.LastOrDefault()))));
                    }
                    else
                    {
                        query.AddPartition(Translator.TranslateExpression(Visit(node.Arguments[1])));
                    }
                }

                if (OrderMethodNames.Contains(method.Name))
                {
                    var sortDirection = SortDirection.Ascending;
                    if (method.Name.Contains("Descending"))
                    {
                        sortDirection = SortDirection.Descending;
                    }
                    var orderExpression = OrderExpression.FromMemberExpression((Expressions.MemberExpression)Translator.TranslateExpression(Visit(node.Arguments[1])), sortDirection);
                    query.AddSort(orderExpression);
                }
            }

            return node.Arguments.Count > 1 ? Visit(node.Arguments[0]) : base.VisitMethodCall(node);
        }

        public ExpressionQuery VisitAndTranslate(Expression expression)
        {
            query = new ExpressionQuery();
            queryToReturn = query;
            Visit(expression);
            return queryToReturn;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var expression = Visit(node.Expression ?? Expression.Constant(null));
            var constantExpression = expression as ConstantExpression;
            if (constantExpression == null)
            {
                return base.VisitMember(node);
            }

            var obj = constantExpression.Value;
            switch (node.Member.MemberType)
            {
                case MemberTypes.Field:
                    return Expression.Constant(((FieldInfo)node.Member).GetValue(obj));
                case MemberTypes.Property:
                    return Expression.Constant(((PropertyInfo)node.Member).GetValue(obj));
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            var test = base.Visit(node.Test) as ConstantExpression;
            if (test == null)
            {
                return base.VisitConditional(node);
            }

            var result = (bool)test.Value;
            return Visit(result ? node.IfTrue : node.IfFalse); // haha
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = base.Visit(node.Left);
            var right = base.Visit(node.Right);
            if (left is ConstantExpression && right is ConstantExpression)
            {
                var value = Expression.Lambda(node.Update(left, node.Conversion, right))
                    .Compile()
                    .DynamicInvoke();
                return Expression.Constant(value);
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null)
            {
                if (node.Value.GetType().IsGenericType &&
                node.Value.GetType().GetGenericTypeDefinition() == typeof(Records<>))
                {
                    query.AddType(node.Value.GetType().GenericTypeArguments[0]);
                }
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var parameters = VisitAndConvert(node.Parameters, node.Name);
            var body = VisitAndConvert(node.Body, node.Name);
            return base.VisitLambda(Expression.Lambda<T>(body ?? node.Body, parameters));
        }

        protected override Expression VisitParameter(System.Linq.Expressions.ParameterExpression node)
        {
            query.AddType((TypeExpression)Translator.TranslateExpression(node));
            return base.VisitParameter(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.Operand);
            while (operand.NodeType == ExpressionType.Quote)
            {
                operand = base.Visit(((UnaryExpression)operand).Operand);
            }

            var lambdaOperand = operand as LambdaExpression;
            if (lambdaOperand != null && lambdaOperand.Parameters.Count == 1 && lambdaOperand.Body == lambdaOperand.Parameters[0])
            {
                return VisitParameter(lambdaOperand.Parameters[0]);
            }

            return base.VisitUnary(node);
        }

        private static ExpressionQuery LastQuery(ExpressionQuery firstQuery)
        {
            var query = firstQuery;
            while (null != query.SubQuery as ExpressionQuery)
            {
                query = query.SubQuery as ExpressionQuery;
            }

            return query;
        }
    }
}