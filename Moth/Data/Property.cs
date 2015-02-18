namespace Moth.Data
{
    using System;

    public class Property
    {
        private object value;

        public Property(string name, object value)
        {
            Type = typeof(object);
            Value = value;
            Name = name;
        }

        public Property(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public string Name { get; set; }

        public Type Type { get; private set; }

        public object Value
        {
            get
            {
                return value;
            }

            set
            {
                if (Type.IsValueType && value == null)
                {
                    Type = typeof (Nullable<>).MakeGenericType(Type);
                }

                this.value = value;
            }
        }

        public int HashCode
        {
            get
            {
                return value.GetHashCode();
            }
        }
    }
}