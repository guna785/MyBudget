using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Contract
{
    public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
    {
    }

    public interface IAuditableEntity : IEntity
    {
        string? CreatedBy { get; set; }

        DateTime? CreatedOn { get; set; }

        string? LastModifiedBy { get; set; }

        DateTime? LastModifiedOn { get; set; }
        string? IPAddress { get; set; }

        bool IsDeleted { get; set; }
    }
}
