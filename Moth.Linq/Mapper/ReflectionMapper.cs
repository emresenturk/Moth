using System;
using System.ComponentModel;
using System.Reflection;
using Moth.Data;

namespace Moth.Linq.Mapper
{
    public class ReflectionMapper : IMapper
    {
        public Entity CreateEntity<T>(T obj) where T : class
        {
            var properties =
                typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            var entityProperties = new Property[properties.Length];
            var index = 0;
            foreach (var property in properties)
            {
                var isOne = property.PropertyType.IsGenericType &&
                            property.PropertyType.GetGenericTypeDefinition() == typeof (One<>);
                var propVal = isOne ? property.PropertyType.GetProperty("UId").GetValue(property.GetValue(obj)) : property.GetValue(obj);
                entityProperties[index] = new Property(property.Name, propVal);    
                
                index++;
            }

            return new Entity(entityProperties);
        }

        public T CreateObject<T>(Entity entity)
        {
            var type = typeof (T);
            return (T)CreateObject(type, entity);
        }

        public object CreateObject(Type type, Entity entity)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            var instance = Activator.CreateInstance(type);
            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof (One<>))
                {
                    var oneInstance = Activator.CreateInstance(property.PropertyType);
                    TypeDescriptor.GetProperties(oneInstance)["UId"].SetValue(oneInstance,entity[property.Name]);
                    property.SetValue(instance, oneInstance);
                }
                else
                {
                    property.SetValue(instance, entity[property.Name]);
                }
            }

            return instance;
        }
    }
}