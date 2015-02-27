using System;
using System.ComponentModel;
using System.Data.Common;

namespace Moth.Linq
{
    public abstract class RecordBase : IModel
    {
        public Guid UId { get; internal set; }

        public int Id { get; internal set; }

        public DateTime DateCreated { get; internal set; }

        public DateTime? DateUpdated { get; internal set; }        
    }

    public abstract class RecordBase<T> : RecordBase where T : class, IModel
    {
        public static Records<T> Records
        {
            get {return new Records<T>();}
        }
            
        public void Create()
        {
            CreateRecord();
            OnCreated(this as T);
        }

        public bool TryCreate(out DbException exception)
        {
            try
            {
                CreateRecord();
            }
            catch (DbException ex)
            {
                exception = ex;
                return false;
            }

            OnCreated(this as T);
            exception = null;
            return true;
        }

        public void Update()
        {
            UpdateRecord();
            OnUpdated(this as T);
        }

        public bool TryUpdate(out DbException exception)
        {
            try
            {
                UpdateRecord();
            }
            catch (DbException ex)
            {
                exception = ex;
                return false;
            }

            OnUpdated(this as T);
            exception = null;
            return true;
        }

        public void Delete()
        {
            DeleteRecord();
            OnDeleted(this as T);
        }

        public bool TryDelete(out DbException exception)
        {
            try
            {
                DeleteRecord();
            }
            catch (DbException ex)
            {
                exception = ex;
                return false;
            }

            OnDeleted(this as T);
            exception = null;
            return true;
        }

        protected virtual void OnCreated(T record)
        {
        }

        protected virtual void OnUpdated(T record)
        {
        }

        protected virtual void OnDeleted(T record)
        {
        }

        private void CreateRecord()
        {
            using (var executor = new ExpressionExecutor())
            {
                var newRecord = executor.Create(this as T);
                var properties = TypeDescriptor.GetProperties(newRecord);
                foreach (PropertyDescriptor property in properties)
                {
                    var propertyValue = property.GetValue(newRecord);
                    if (propertyValue != null)
                    {
                        property.SetValue(this, propertyValue);
                    }
                }

                UId = newRecord.UId;
                Id = newRecord.Id;
            }
        }

        private void UpdateRecord()
        {
            using (var executor = new ExpressionExecutor())
            {
                var updatedRecord = executor.Update(this as T);
                var properties = TypeDescriptor.GetProperties(updatedRecord);
                foreach (PropertyDescriptor property in properties)
                {
                    var propertyValue = property.GetValue(updatedRecord);
                    if (propertyValue != null)
                    {
                        property.SetValue(this, propertyValue);
                    }
                }
            }
        }

        private void DeleteRecord()
        {
            using (var executor = new ExpressionExecutor())
            {
                executor.Delete(this as T);
            }
        }
    }
}