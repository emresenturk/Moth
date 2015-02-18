using System;
using Moth.Annotations;
using Moth.Configuration;

namespace Moth.Database
{
    public interface IDatabaseContainer : IDisposable
    {
        bool HasRegisteredItems { get; }
        void Register<T>() where T : class, IDatabase, new();
        void Register(Type databaseType);
        T GetInstance<T>() where T : class, IDatabase, new ();
        IDatabase GetInstance(Type databaseType);
        IDatabase GetInstance();
        void Register<T>([NotNull]IDatabaseConfiguration configuration) where T : class, IDatabase, new();
        void Register(Type databaseType, IDatabaseConfiguration configuration);
        T GetInstance<T>([NotNull]string name) where T : class, IDatabase, new();
        IDatabase GetInstance(Type databaseType, [NotNull]string name);
        T GetInstance<T>([NotNull]IDatabaseConfiguration configuration) where T : class, IDatabase, new();
        IDatabase GetInstance(Type databaseType, IDatabaseConfiguration configuration);
        IDatabase GetInstance(string name);
    }
}