using System;
using System.Collections.Generic;
using System.Linq;
using Moth.Expressions;
using Moth.Linq.Mapper;

namespace Moth.Linq
{
    internal class ExpressionExecutor : Executor
    {
        private readonly IMapper mapper;

        public ExpressionExecutor()
        {
            mapper = new ReflectionMapper();
        }

        public IEnumerable<T> ExecuteReader<T>(IQuery query)
        {
            return ExecuteReader(query).Select(entity => mapper.CreateObject<T>(entity));
        }

        public IList<T> ExecuteRetrieve<T>(IQuery query)
        {
            return ExecuteRetrieve(query).Select(entity => mapper.CreateObject<T>(entity)).ToList();
        }

        public IEnumerable<Record<T>> ExecuteRecordReader<T>(IQuery query) where T : class
        {
            return ExecuteReader(query).Select(entity => new Record<T>
            {
                Entity = mapper.CreateObject<T>(entity),
                Id = entity.GetValue<int>("Id"),
                UId = entity.GetValue<Guid>("UId"),
                DateCreated = entity.GetValue<DateTime>("DateCreated"),
                DateUpdated = entity.GetValue<DateTime?>("DateUpdated")
            });
        }

        public T Create<T>(T obj) where T : class, IModel
        {
            var typeExpression = new TypeExpression(typeof (T));
            if (obj is RecordBase<T>)
            {
                (obj as RecordBase<T>).DateCreated = DateTime.UtcNow;
                (obj as RecordBase<T>).UId = Guid.NewGuid();

            }

            var createdEntity = Database.Create(mapper.CreateEntity(obj), typeExpression);
            obj = mapper.CreateObject<T>(createdEntity);
            return obj;
        }

        public T Update<T>(T obj) where T : class, IModel
        {
            var typeExpression = new TypeExpression(typeof(T));
            if (obj is RecordBase<T>)
            {
                (obj as RecordBase<T>).DateUpdated = DateTime.UtcNow;
            }

            var updatedEntity = Database.Update(mapper.CreateEntity(obj), typeExpression);
            var updatedObj = mapper.CreateObject<T>(updatedEntity);
            return updatedObj;
        }

        public T Delete<T>(T obj) where T : class, IModel
        {
            var typeExpression = new TypeExpression(typeof (T));
            var deletedEntity = Database.Delete(mapper.CreateEntity(obj), typeExpression);
            return mapper.CreateObject<T>(deletedEntity);
        }
    }
}