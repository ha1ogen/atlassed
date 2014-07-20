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
        public override string _className { get { return ClassName; } }
        public override int ObjectId { get { return EntityId; } }
        public int EntityId { get; set; }
        public string ClassName { get; set; }
        public int MapId { get; set; }
        public IEnumerable<Coordinate> EntityCoordinates { get; set; }
        public string MapLabel { get; set; }
    }

    public class MapEntityValidator : IValidatorWNew<MapEntity, MapEntity>
    {
        private IValidator<MetaObject> _metaObjectValidator;

        public MapEntityValidator(IValidator<MetaObject> metaObjectValidator)
        {
            _metaObjectValidator = metaObjectValidator;
        }

        public bool Validate(MapEntity record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (record.EntityId == 0)
                result.AddError("EntityId", "Entity ID is required");

            return result.IsValid();
        }


        public bool ValidateNew(MapEntity record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (string.IsNullOrEmpty(record.ClassName))
                result.AddError("ClassName", "Class Name is required");

            if (record.MapId == 0)
                result.AddError("MapId", "Map ID is required");

            if (record.EntityCoordinates == null || record.EntityCoordinates.Count() == 0)
                result.AddError("EntityCoordinates", "At least one entity coordinate must be supplied");

            return result.IsValid();
        }

        private bool ValidateCommon(MapEntity record, out IValidationResult result)
        {
            return _metaObjectValidator.Validate(record, out result);
        }
    }
}