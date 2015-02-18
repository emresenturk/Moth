using System;

namespace Moth.Expressions
{
    public class ConstantExpression : IQueryExpression
    {
        public ConstantExpression()
        {
        }

        public ConstantExpression(object value)
        {
            Value = value;
        }

        public object Value { get; set; }

        public Type ValueType
        {
            get
            {
                return Value.GetType();
            }
        }
    }
}