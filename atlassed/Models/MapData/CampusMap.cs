using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class CampusMap : Map
    {
        public override string _className { get { return typeof(CampusMap).Name; } }
        public string CampusName { get; set; }
        public Tuple<Coordinate, Coordinate> MapCoordinates { get; set; }
    }

    public class CampusMapValidator : IValidatorWNew<CampusMap, CampusMap>
    {
        private IValidator<MetaObject> _metaObjectValidator;

        public CampusMapValidator(IValidator<MetaObject> metaObjectValidator)
        {
            _metaObjectValidator = metaObjectValidator;
        }

        public bool Validate(CampusMap record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (record.MapId == 0)
                result.AddError("MapId", "Map ID is required");

            return result.IsValid();
        }

        public bool ValidateNew(CampusMap record, out IValidationResult result)
        {
            return ValidateCommon(record, out result);
        }

        private bool ValidateCommon(CampusMap record, out IValidationResult result)
        {
            _metaObjectValidator.Validate(record, out result);

            if (string.IsNullOrEmpty(record.CampusName))
                result.AddError("CampusName", "Campus Name is required");

            if (record.MapCoordinates == null)
                result.AddError("MapCoordinates", "Map Coordinates are required");

            return result.IsValid();
        }
    }
}