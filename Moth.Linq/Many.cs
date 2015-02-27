using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Moth.Linq
{
    public class Many<T> : Records<T> where T : class, IModel
    {
        public Many() : base()
        {            
        }

        public Many(string relationName, Guid uId)
        {
            RelationName = relationName;
            UId = uId;
            var variableExpression = Expression.Variable(ElementType, "p");
            var memberInfo = ElementType.GetMember(RelationName)[0];
            var memberAccess = Expression.MakeMemberAccess(variableExpression, memberInfo);
            var parentExpression = Expression.Equal(memberAccess, Expression.Constant(UId));
            var whereExpression = Expression.Call(typeof(Queryable), "Where", new Type[] { typeof(T) },
                Expression,
                Expression.Lambda<Func<T, bool>>(parentExpression, Expression.Parameter(typeof(T), "p")));
            Expression = whereExpression;
        }

        private string RelationName { get; set; }
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

        public void Remove(T entity)
        {
            var relationProperty = TypeDescriptor.GetProperties(entity)[RelationName];
            if (relationProperty != null)
            {
                var relationObject = relationProperty.GetValue(entity);
                if (relationObject != null)
                {
                    TypeDescriptor.GetProperties(relationObject)["UId"].ResetValue(relationObject);
                }

                if (entity.UId != Guid.Empty && entity is RecordBase<T>)
                {
                    (entity as RecordBase<T>).Update();
                }
            }
        }
    }
}