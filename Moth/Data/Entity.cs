using System.Linq;

namespace Moth.Data
{
    using System;
    using System.Collections.Generic;

    public class Entity
    {        
        private readonly Property[] properties;

        public Entity(Dictionary<string, object> properties)
        {
            this.properties = new Property[properties.Count];
            PropertyNames = properties.Keys.ToArray();
            var index = 0;
            foreach (var property in properties)
            {
                this.properties[index] = new Property(property.Key, property.Value.GetType(), property.Value);
                index++;
            }
        }

        public Entity(params Property[] properties)
        {
            this.properties = properties;
            PropertyNames = new string[properties.Length];
            for (var index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                PropertyNames[index] = property.Name;
            }
        }

        public string[] PropertyNames { get; private set; }

        public object this[int index]
        {
            get
            {
                return properties[index].Value;
            }

            set
            {
                SetValue(index, value);
            }
        }

        public object this[string name]
        {
            get
            {
                var index = IndexOf(name);
                if (index == -1)
                {
                    throw new ArgumentException(string.Format(@"Property with name ""{0}"" not found", name), "name");
                }

                return this[index];
            }

            set
            {
                SetValue(name, value.GetType(), value);
            }
        }

        public T GetValue<T>(string name)
        {
            var value = this[name];
            return (T)value;
        }

        public void SetValue<T>(string name, T value)
        {
            SetValue(name, typeof(T), value);
        }

        private void SetValue(int index, object value)
        {
            properties[index].Value = value;
        }

        private void SetValue(string name, Type type, object value)
        {
            var index = IndexOf(name);
            if (index != -1)
            {
                properties[index].Value = Convert.ChangeType(value, type);
            }
        }

        public override int GetHashCode()
        {
            return (properties != null ? properties.GetHashCode() : 0);
        }

        private int IndexOf(string name)
        {
            for (var index = 0; index < properties.Length; index++)
            {
                if (properties[index].Name == name)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}