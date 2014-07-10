using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlassed.Repositories
{
    interface ISearchableRepository<T, NT, ID, PID, ST> : IRepository<T, NT, ID, PID>
    {
        IEnumerable<ST> Search(string query, int skip, int? take);
    }
}
