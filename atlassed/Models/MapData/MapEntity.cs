using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class MapEntity : MetaObject
    {
        public int EntityId { get; set; }
        public string ClassName { get; set; }
        public int MapId { get; set; }
        public List<Coordinate> EntityCoordinates { get; set; }
        public string MapLabel { get; set; }
    }

    public class MapEntityValidator : IValidator<MapEntity>
    {
        public bool Validate(MapEntity record, out IEnumerable<ValidationError> errors)
        {
            errors = new List<ValidationError>();
            return true;
        }
    }
}