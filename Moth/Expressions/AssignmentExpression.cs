namespace Moth.Expressions
{
    public class AssignmentExpression : IQueryExpression
    {
        public IQueryExpression Destination { get; set; }

        public IQueryExpression Source { get; set; }
    }
}