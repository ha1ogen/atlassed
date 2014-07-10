using Atlassed.Repositories.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class MetaClass
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassType { get; set; }
        public string ClassTypeDescription { get; set; }
        public string ClassLabel { get; set; }
        public IEnumerable<MetaField> MetaFields { get; set; }
    }
}