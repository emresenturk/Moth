using System;

namespace Moth.Expressions
{
    public class TypeExpression : IQueryExpression
    {
        public TypeExpression()
        {
        }

        public TypeExpression(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; set; }

    }
}