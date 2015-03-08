using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Moth.Expressions;
using SystemExpressions = System.Linq.Expressions;

namespace Moth.Linq
{
    internal static class Translator
    {
        public static IQueryExpression TranslateExpression(SystemExpressions.Expression expression)
        {
            var exp = expression;
            var unaryExpression = exp as SystemExpressions.UnaryExpression;
            if (unaryExpression != null)
            {
                while (exp.NodeType == SystemExpressions.ExpressionType.Quote)
                {
                    exp = ((SystemExpressions.UnaryExpression)exp).Operand;
                    
                }
            }

            if (exp.NodeType == SystemExpressions.ExpressionType.Lambda)
            {
                exp = ((SystemExpressions.LambdaExpression)exp).Body;
            }

            if (exp.NodeType.IsBinary())
            {
                return TranslateBinaryExpression((SystemExpressions.BinaryExpression)exp);
            }

            if (exp.NodeType == SystemExpressions.ExpressionType.Constant)
            {
                return TranslateConstantExpression((SystemExpressions.ConstantExpression)exp);
            }

            if (exp.NodeType == SystemExpressions.ExpressionType.MemberAccess)
            {
                return TranslateMemberAccessExpression((SystemExpressions.MemberExpression)exp);
            }

            if (exp.NodeType == SystemExpressions.ExpressionType.Parameter)
            {
                return new TypeExpression(exp.Type);
            }

            if (exp is SystemExpressions.UnaryExpression)
            {
                throw new NotSupportedException((exp as SystemExpressions.UnaryExpression).Method.Name);
            }

            if (exp is SystemExpressions.MethodCallExpression)
            {
                throw new NotSupportedException((expression as SystemExpressions.MethodCallExpression).Method.Name);
            }

            throw new NotImplementedException(string.Format("Expression Type: {0}, Expression: {1}", expression.NodeType, expression));
        }

        private static MemberExpression TranslateMemberAccessExpression(SystemExpressions.MemberExpression memberExpression)
        {
            if (!memberExpression.Member.MemberType.HasFlag(MemberTypes.Property))
            {
                throw new NotSupportedException("Only property type is supported for member access");
            }
            var property = (PropertyInfo)memberExpression.Member;
            var objectType = memberExpression.Expression.Type.IsGenericType && memberExpression.Expression.Type.GetGenericTypeDefinition() == typeof(RecordBase<>)
                ? memberExpression.Expression.Type.GenericTypeArguments[0]
                : memberExpression.Expression.Type;
            
            return new MemberExpression(memberExpression.Member.Name, property.PropertyType, objectType);
        }

        private static ParameterExpression TranslateConstantExpression(SystemExpressions.ConstantExpression constantExpression)
        {
            return new ParameterExpression(new Parameter(constantExpression.Value));
        }

        private static BinaryExpression TranslateBinaryExpression(SystemExpressions.BinaryExpression binaryExpression)
        {
            var left = TranslateExpression(binaryExpression.Left);
            var right = TranslateExpression(binaryExpression.Right);
            var @operator = TranslateBinaryOperator(binaryExpression.NodeType);
            return new BinaryExpression(left, @operator, right);
        }

        private static BinaryOperator TranslateBinaryOperator(SystemExpressions.ExpressionType nodeType)
        {
            var nodeTypeName = Enum.GetName(typeof(SystemExpressions.ExpressionType), nodeType);
            BinaryOperator binaryOperator;
            if (Enum.TryParse(nodeTypeName, out binaryOperator))
            {
                return binaryOperator;
            }

            throw new ArgumentOutOfRangeException("nodeType", nodeType, nodeTypeName);
        }

        private static bool IsBinary(this SystemExpressions.ExpressionType nodeType)
        {
            return new[]
            {
                SystemExpressions.ExpressionType.Add,
                SystemExpressions.ExpressionType.Divide,
                SystemExpressions.ExpressionType.Subtract,
                SystemExpressions.ExpressionType.Multiply,
                SystemExpressions.ExpressionType.Power,
                SystemExpressions.ExpressionType.Modulo,
                SystemExpressions.ExpressionType.And,
                SystemExpressions.ExpressionType.Or,
                SystemExpressions.ExpressionType.ExclusiveOr,
                SystemExpressions.ExpressionType.AndAlso,
                SystemExpressions.ExpressionType.OrElse,
                SystemExpressions.ExpressionType.Equal,
                SystemExpressions.ExpressionType.NotEqual,
                SystemExpressions.ExpressionType.GreaterThanOrEqual,
                SystemExpressions.ExpressionType.GreaterThan,
                SystemExpressions.ExpressionType.LessThan,
                SystemExpressions.ExpressionType.LessThanOrEqual
            }.Contains(nodeType);
        }
    }
}