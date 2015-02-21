using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Moth.Linq
{
    public class Many<T> : Records<T> where T : class, IModel
    {
        private Expression parentExpression;

        public Many() : base()
        {
            ((RecordProvider<T>)Provider).OnBeforeExecute += (sender, args) =>
            {
                parentExpression = Expression.Equal(Expression.MakeMemberAccess(Expression.Variable(typeof(T), "p"), typeof(T).GetMember(RelationName)[0]), Expression.Constant(UId));
                var whereExpression = Expression.Call(typeof (Queryable), "Where", new Type[] {typeof (T)},
                    Expression,
                    Expression.Lambda<Func<T, bool>>(parentExpression, Expression.Parameter(typeof (T), "p")));
                args.Expression = whereExpression;
            };
        }

        public string RelationName { get; set; }
        public Guid UId { get; set; }

        public void Add(T entity)
        {
            var relationProperty = TypeDescriptor.GetProperties(entity)[RelationName];
            if (relationProperty != null)
            {
                var relationObject = relationProperty.GetValue(entity);
                if (relationObject != null)
                {
                    TypeDescriptor.GetProperties(relationObject)["UId"].SetValue(relationObject, UId);
                }
            }

            if (entity.UId != Guid.Empty && entity is RecordBase<T>)
            {
                (entity as RecordBase<T>).Update();
            }
        }

    }
}