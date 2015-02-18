using System;
using System.Collections.Generic;
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
            foreach (var propertyInfo in properties)
            {
                entityProperties[index] = new Property(propertyInfo.Name, propertyInfo.GetValue(obj));
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
            foreach (var propertyInfo in properties)
            {
                propertyInfo.SetValue(instance, entity[propertyInfo.Name]);
            }

            return instance;
        }
    }
}