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
        private readonly string[] queryMethodNames =
        {
            "Where", "Select", "OrderBy", "OrderByDescending", "ThenBy",
            "ThenByDescending","First","FirstOrDefault","Single","SingleOrDefault"
        };

        private ExpressionQuery query;

        public ExpressionQuery VisitAndTranslate(Expression expression)
        {
            query = new ExpressionQuery();
            this.Visit(expression);
            return query;
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
                    return Expression.Constant(((FieldInfo) node.Member).GetValue(obj));
                case MemberTypes.Property:
                    return Expression.Constant(((PropertyInfo) node.Member).GetValue(obj));
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

            var result = (bool) test.Value;
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

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof (Queryable) && queryMethodNames.Contains(node.Method.Name))
            {
                switch (node.Method.Name)
                {
                    case "First":
                    case "FirstOrDefault":
                    case "Single":
                    case "SingleOrDefault":
                    case "Where":
                        query.AddFilter(Translator.TranslateExpression(Visit(node.Arguments[1])));
                        break;
                    case "OrderBy":
                    case "ThenBy":
                        query.AddOrder(OrderExpression.FromMemberExpression((Expressions.MemberExpression) Translator.TranslateExpression(Visit(node.Arguments[1])), OrderDirection.Ascending));
                        break;
                    case "OrderByDescending":
                    case "ThenByDescending":
                        query.AddOrder(OrderExpression.FromMemberExpression((Expressions.MemberExpression)Translator.TranslateExpression(Visit(node.Arguments[1])), OrderDirection.Descending));
                        break;
                    default:
                        Trace.WriteLine(node.Method.Name);
                        break;
                }
                return base.Visit(node.Arguments[0]);
            }

            var argumentExpressions = node.Arguments.Select(Visit).ToList();
            var objectExpression = Visit(node.Object ?? Expression.Constant(null)) as ConstantExpression;
            if (objectExpression != null && objectExpression.Value != null && (argumentExpressions.All(e => e.NodeType == ExpressionType.Constant)))
            {
                var value = node.Method.Invoke(objectExpression.Value,
                    argumentExpressions.Cast<ConstantExpression>().Select(e => e.Value).ToArray());
                return Expression.Constant(value);
            }
            return base.VisitMethodCall(node);
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var parameters = VisitAndConvert(node.Parameters, node.Name);
            var body = VisitAndConvert(node.Body, node.Name);
            return base.VisitLambda(Expression.Lambda<T>(body ?? node.Body, parameters));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.Operand);
            while (operand.NodeType == ExpressionType.Quote)
            {
                operand = base.Visit(((UnaryExpression) operand).Operand);
            }
            
            if (operand != node.Operand)
            {
                return node.Update(operand);
            }

            return base.VisitUnary(node);
        }
    }
}