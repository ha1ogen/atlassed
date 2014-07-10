using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class MapEntityClass : MetaClass
    {
        private const string _iconFilenameExtension = ".png";

        public string MapLabelFieldName { get; set; }
        public string IconFilename { get { return "icon-" + ClassId + _iconFilenameExtension; } }
    }

    public class MapEntityClassValidator : IValidator<MapEntityClass>
    {
        public bool Validate(MapEntityClass record, out ICollection<ValidationError> errors)
        {
            errors = new List<ValidationError>();
            return true;
        }
    }
}