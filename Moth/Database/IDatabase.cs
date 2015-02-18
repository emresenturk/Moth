using System;
using System.Collections.Generic;
using Moth.Configuration;
using Moth.Data;
using Moth.Expressions;

namespace Moth.Database
{
    public interface IDatabase : IDisposable
    {
        IDatabaseConfiguration Configuration { get; }

        IList<Entity> Retrieve(IQuery query);

        IEnumerable<Entity> Read(IQuery query);

        Entity Create(Entity entity, TypeExpression entityType);

        Entity Update(Entity entity, TypeExpression entityType);

        Entity Delete(Entity entity, TypeExpression entityType);

        int NonQuery(IQuery query);
    }
}