using System;

namespace Moth.Linq
{
    public interface IModel
    {
        Guid UId { get; }
        int Id { get; }
        DateTime DateCreated { get; }
        DateTime? DateUpdated { get; }
    }
}