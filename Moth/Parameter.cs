using System;
using Moth.Annotations;

namespace Moth
{
    public class Parameter
    {
        public Parameter()
        {
        }

        public Parameter(object value)
        {
            Value = value;
        }

        public Parameter([NotNull]string name, object value)
        {
            Name = name;
            Value = value;
            ValueType = value != null ? value.GetType() : null;
        }

        public Parameter([NotNull]string name, object value, Type valueType)
            : this(name, value)
        {
            //if (value == null || value.GetType() != valueType)
            //{
            //    throw new ArgumentException("value type and type of value are not equal", "valueType");
            //}

            ValueType = valueType;
        }

        public string Name { get; set; }

        public object Value { get; private set; }

        public Type ValueType { get; private set; }
    }
}