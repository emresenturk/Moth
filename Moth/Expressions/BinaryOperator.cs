namespace Moth.Expressions
{
    public enum BinaryOperator
    {
        // Arithmatic
        Add,
        Divide,
        Subtract,
        Multiply,
        Power,
        Modulo,

        // Bitwise
        And,
        Or,
        ExclusiveOr,

        // Logical
        AndAlso,
        OrElse,

        // Comparison
        Equal,
        NotEqual,
        GreaterThanOrEqual,
        GreaterThan,
        LessThan,
        LessThanOrEqual
    }
}