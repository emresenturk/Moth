namespace Moth.Linq
{
    public class Record<T> : RecordBase<Record<T>> where T : class
    {
        public T Entity { get; set; }

        public static implicit operator T(Record<T> record)
        {
            return record.Entity;
        }

        public static implicit operator Record<T>(T entity)
        {
            return new Record<T> {Entity = entity};
        }
    }
}