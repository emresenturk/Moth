using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using Moth.Data;
using Moth.Linq.Attributes;

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
                    if (property.PropertyType.IsGenericType)
                    {
                        var propertyGenericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                        if (propertyGenericTypeDefinition == typeof(One<>))
                        {
                            var oneInstance = Activator.CreateInstance(property.PropertyType);
                            TypeDescriptor.GetProperties(oneInstance)["UId"].SetValue(oneInstance, entity[property.Name]);
                            property.SetValue(instance, oneInstance);
                        }
                        else if (propertyGenericTypeDefinition == typeof(Many<>))
                        {
                            var propertyAttribute = property.GetCustomAttribute<OneToManyAttribute>();
                            var relationName = propertyAttribute != null ? propertyAttribute.Relation : type.Name;
                            var manyInstance = Activator.CreateInstance(property.PropertyType, relationName, entity["UId"]);
                            property.SetValue(instance, manyInstance);
                        }
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
                var entityProperties = new Property[properties.Length];
                var index = 0;
                foreach (var property in properties)
                {
                    var isOne = false;
                    if (property.PropertyType.IsGenericType)
                    {
                        var propertyGenericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                        if (propertyGenericTypeDefinition == typeof(One<>))
                        {
                            isOne = true;
                        }
                        else if (propertyGenericTypeDefinition == typeof(Many<>))
                        {
                            continue;
                        }
                    }

                    var propVal = property.GetValue(obj);
                    if (isOne && propVal != null)
                    {
                        propVal = property.PropertyType.GetProperty("UId").GetValue(propVal);
                    }
                    entityProperties[index] = new Property(property.Name, propVal);

                    index++;
                }

                return new Entity(entityProperties);
            };
        }
    }
}