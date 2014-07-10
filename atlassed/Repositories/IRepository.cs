using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlassed.Repositories
{
    interface IRepository<T, NT, ID, PID>
    {
        T Create(NT record, out ICollection<ValidationError> errors);
        bool Update(ref T record, out ICollection<ValidationError> errors);
        bool Delete(ID recordId);
        T GetOne(ID recordId);
        IEnumerable<T> GetMany(PID parentId = default(PID));
    }
}
