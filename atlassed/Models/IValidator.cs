using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlassed.Models
{
    public interface IValidator<T>
    {
        bool Validate(T record, out IEnumerable<ValidationError> errors);
    }
}
