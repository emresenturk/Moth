namespace Moth.Expressions
{
    public class ParameterExpression : IQueryExpression
    {
        public ParameterExpression()
        {
        }

        public ParameterExpression(Parameter parameter)
        {
            Parameter = parameter;
        }

        public Parameter Parameter { get; set; }
    }
}