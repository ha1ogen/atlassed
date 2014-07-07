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
        public const string _entityId = "entityId";
        public const string _entityPoints = "entityPoints";

        private const string _spAddMapEntity = "AddMapEntity";
        private const string _spEditMapEntity = "EditMapEntity";
        private const string _spDeleteMapEntity = "DeleteMapEntity";
        private const string _spGetMapEntities = "GetMapEntities";
        private const string _spSearchMapEntities = "SearchMapEntities";

        public int EntityId { get; set; }
        public string ClassName { get; set; }
        public int MapId { get; set; }
        public List<Point> EntityPoints { get; set; }
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