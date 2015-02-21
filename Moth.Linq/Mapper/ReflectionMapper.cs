using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Moth.Data;
using Moth.Linq.Attributes;

namespace Moth.Linq.Mapper
{
    public class ReflectionMapper : IMapper
    {
        public Entity CreateEntity<T>(T obj) where T : class
        {
            var properties =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty).Where(p => !p.PropertyType.IsGenericType || (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() != typeof(Many<>))).ToArray();
            var entityProperties = new Property[properties.Length];
            var index = 0;
            foreach (var property in properties)
            {
                var isOne = false;
                if (property.PropertyType.IsGenericType)
                {
                    var propertyGenericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                    if ( propertyGenericTypeDefinition == typeof (One<>))
                    {
                        isOne = true;
                    }
                    else if (propertyGenericTypeDefinition == typeof (Many<>))
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
                if (property.PropertyType.IsGenericType)
                {
                    var propertyGenericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                    if (propertyGenericTypeDefinition == typeof (One<>))
                    {
                        var oneInstance = Activator.CreateInstance(property.PropertyType);
                        TypeDescriptor.GetProperties(oneInstance)["UId"].SetValue(oneInstance, entity[property.Name]);
                        property.SetValue(instance, oneInstance);
                    }
                    else if (propertyGenericTypeDefinition == typeof (Many<>))
                    {
                        var propertyAttribute = property.GetCustomAttribute<OneToManyAttribute>();                        
                        var manyInstance = Activator.CreateInstance(property.PropertyType);
                        if (propertyAttribute != null)
                        {
                            TypeDescriptor.GetProperties(manyInstance)["RelationName"].SetValue(manyInstance,
                                propertyAttribute.RelationName);
                        }
                        else
                        {
                            TypeDescriptor.GetProperties(manyInstance)["RelationName"].SetValue(manyInstance, type.Name);
                        }
                        TypeDescriptor.GetProperties(manyInstance)["UId"].SetValue(manyInstance, entity["UId"]);
                        property.SetValue(instance, manyInstance);
                    }
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