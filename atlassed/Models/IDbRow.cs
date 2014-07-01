using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlassed.Models
{
    interface IDbRow<T>
    {
        T CommitUpdate();
        bool Delete();
    }
}
