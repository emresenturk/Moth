using System;
using Moth.Data;

namespace Moth.Linq.Mapper
{
    public interface IMapper
    {
        Entity CreateEntity<T>(T obj) where T : class;
        T CreateObject<T>(Entity entity);
        object CreateObject(Type type, Entity entity);
    }
}