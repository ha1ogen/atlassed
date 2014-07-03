using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlassed.Models.MapData
{
    public interface ISearchable
    {
        SearchResult ToSearchResult();
    }
}
