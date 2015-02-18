namespace Moth.Expressions
{
    public class BinaryExpression : IQueryExpression
    {
        public BinaryExpression()
        {
        }

        public BinaryExpression(IQueryExpression left, BinaryOperator @operator, IQueryExpression right)
        {
            Left = left;
            Right = right;
            Operator = @operator;
        }

        public IQueryExpression Left { get; set; }
        public IQueryExpression Right { get; set; }
        public BinaryOperator Operator { get; set; }
    }
}