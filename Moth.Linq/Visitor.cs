using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
            "First",
            "FirstOrDefault",
            "Last",
            "LastOrDefault",
            "Max",
            "Min",
            "Single",
            "SingleOrDefault",
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
        
        private ExpressionQuery query;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            if (QueryMethodNames.Contains(node.Method.Name))
            {
                if (FilterMethodNames.Contains(method.Name) && node.Arguments.Count > 1)
                {
                    // Add Filter    
                }

                if (AggregateMethodNames.Contains(method.Name) && node.Arguments.Count < 2)
                {
                    // Add aggregate
                }

                if (ProjectionMethodNames.Contains(method.Name) && node.Arguments.Count > 1)
                {
                    // Convert current query to subquery of new query
                    // Add projection
                }

            }
            Trace.WriteLine(string.Format("Method Name: {0}", node.Method.Name));
            Trace.WriteLine("Arguments:");
            Trace.WriteLine("-----------------------------");
            for (var index = 0; index < node.Arguments.Count; index++)
            {
                var expression = node.Arguments[index];
                Trace.WriteLine(string.Format("{0}\t{1}", index, expression));
            }
            Trace.WriteLine("=============================");
            Trace.WriteLine("");
            Trace.WriteLine("");
            return base.VisitMethodCall(node);
        }

        public ExpressionQuery VisitAndTranslate(Expression expression)
        {
            query = new ExpressionQuery();
            Visit(expression);
            return query;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value.GetType().IsGenericType &&
                node.Value.GetType().GetGenericTypeDefinition() == typeof (Records<>))
            {
                query.AddType(node.Value.GetType().GenericTypeArguments[0]);
                Trace.WriteLine("");
                Trace.WriteLine(string.Format("ConstantExpression: {0}, ValueType:{1}", node, node.Value.GetType()));
                Trace.WriteLine("");
            }
            
            return base.VisitConstant(node);
        }

        public override Expression Visit(Expression node)
        {
            if (node != null)
            {
                Trace.WriteLine(string.Format("{0}: {1}", node.NodeType, node));
            }
            return base.Visit(node);
        }
    }
}