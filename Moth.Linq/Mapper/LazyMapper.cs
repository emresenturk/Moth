using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Moth.Data;

namespace Moth.Linq.Mapper
{
    public class LazyMapper : IMapper
    {
        private static readonly ConcurrentDictionary<Type, Func<Entity, object>> Constructors;

        private static readonly ConcurrentDictionary<Type, Func<object, Entity>> EntityConverters; 

        static LazyMapper()
        {
            Constructors = new ConcurrentDictionary<Type, Func<Entity, object>>();
            EntityConverters = new ConcurrentDictionary<Type, Func<object, Entity>>();
        }

        public Entity CreateEntity<T>(T obj) where T : class
        {
            var type = typeof (T);
            return EntityConverters.GetOrAdd(type, CreateEntityConverter)(obj);
        }

        public Entity CreateEntity(Type type, object obj)
        {
            return EntityConverters.GetOrAdd(type, CreateEntityConverter)(obj);
        }

        public T CreateObject<T>(Entity entity)
        {
            var type = typeof (T);
            return (T) Constructors.GetOrAdd(type, CreateConstructor)(entity);
        }

        public object CreateObject(Type type, Entity entity)
        {
            return Constructors.GetOrAdd(type, CreateConstructor)(entity);
        }

        private Func<Entity, object> CreateConstructor(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            return entity =>
            {
                var instance = Activator.CreateInstance(type);
                foreach (var property in properties)
                {
                    if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(One<>))
                    {
                        var oneInstance = Activator.CreateInstance(property.PropertyType);
                        TypeDescriptor.GetProperties(oneInstance)["UId"].SetValue(oneInstance, entity[property.Name]);
                        property.SetValue(instance, oneInstance);
                    }
                    else
                    {
                        property.SetValue(instance, entity[property.Name]);
                    }
                }
                return instance;
            };
        }

        private Func<object, Entity> CreateEntityConverter(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            return obj =>
            {
                var entityProperties = new Dictionary<string, object>();
                foreach (var property in properties)
                {
                    var isOne = property.PropertyType.IsGenericType &&
                            property.PropertyType.GetGenericTypeDefinition() == typeof(One<>);
                    var propVal = isOne ? property.PropertyType.GetProperty("UId").GetValue(property.GetValue(obj)) : property.GetValue(obj);
                    entityProperties[property.Name] = new Property(property.Name, propVal);   
                }

                return new Entity(entityProperties);
            };
        }
    }
}