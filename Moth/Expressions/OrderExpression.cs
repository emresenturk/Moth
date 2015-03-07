namespace Moth.Expressions
{
    public class OrderExpression : MemberExpression
    {
        public OrderExpression()
        {
        }

        public SortDirection Direction { get; set; }

        public static OrderExpression FromMemberExpression(MemberExpression expression, SortDirection direction)
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