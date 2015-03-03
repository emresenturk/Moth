namespace Moth.Expressions
{
    public class MethodExpression : IQueryExpression
    {
        public MethodExpression()
        {
        }

        public MethodExpression(MethodType type, IQueryExpression parameter)
        {
            Type = type;
            Parameter = parameter;
        }

        public MethodType Type { get; set; }
        public IQueryExpression Parameter { get; set; }
    }
}