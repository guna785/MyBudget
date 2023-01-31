using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Contract
{
    public interface IEntity<TId> : IEntity
    {
        public TId Id { get; set; }
    }

    public interface IEntity
    {
    }
}
