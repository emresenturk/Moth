using System.Collections.Generic;
using Moth.Data;

namespace Moth.Extensions
{
    public static class ExecutorExtension
    {
        public static Executor On(this Executor source, string databaseName)
        {
            source.SetDatabase(databaseName);
            return source;
        }

        public static int NonQuery(this Executor source)
        {
            return source.ExecuteNonQuery(source.ExtensionQuery);
        }

        public static IEnumerable<Entity> Reader(this Executor source)
        {
            return source.ExecuteReader(source.ExtensionQuery);
        }

        public static IList<Entity> Retrieve(this Executor source)
        {
            return source.ExecuteRetrieve(source.ExtensionQuery);
        }

        public static object Scalar(this Executor source)
        {
            return source.ExecuteScalar(source.ExtensionQuery);
        }

        public static T Scalar<T>(this Executor source) where T : struct
        {
            return source.ExecuteScalar<T>(source.ExtensionQuery);
        }
    }
}