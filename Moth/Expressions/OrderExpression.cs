namespace Moth.Expressions
{
    public class OrderExpression : MemberExpression
    {
        public OrderExpression()
        {
        }

        public OrderDirection Direction { get; set; }

        public static OrderExpression FromMemberExpression(MemberExpression expression, OrderDirection direction)
        {
            return new OrderExpression
                        {
                            MemberName = expression.MemberName,
                            MemberType = expression.MemberType,
                            Namespace = expression.Namespace,
                            ObjectName = expression.ObjectName,
                            ObjectType = expression.ObjectType,
                            Direction = direction
                        };
        }
    }
}