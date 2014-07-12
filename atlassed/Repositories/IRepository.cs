using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlassed.Repositories
{
    interface IExistenceRepository<ID>
    {
        bool RecordExists(ID recordId);
    }

    interface IReadOnlyRepository<T, ID, PID> : IExistenceRepository<ID>
    {
        T GetOne(ID recordId);
        IEnumerable<T> GetMany(PID parentId = default(PID));
    }

    interface IRepository<T, NT, ID, PID> : IReadOnlyRepository<T, ID, PID>
    {
        T Create(NT record, out ICollection<ValidationError> errors);
        bool Update(ref T record, out ICollection<ValidationError> errors);
        bool Delete(ID recordId);
    }

    interface ISearchableRepository<T, NT, ID, PID, ST> : IRepository<T, NT, ID, PID>
    {
        IEnumerable<ST> Search(string query, int skip, int? take);
    }
}
