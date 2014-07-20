using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class FloorMap : Map
    {
        public override string _className { get { return typeof(FloorMap).Name; } }
        public int BuildingId { get; set; }
        public int FloorOrdinal { get; set; }
        public string FloorCode { get; set; }
        public string FloorLabel { get; set; }
    }

    public class FloorMapValidator : IValidatorWNew<FloorMap, FloorMap>
    {
        private IValidator<MetaObject> _metaObjectValidator;

        public FloorMapValidator(IValidator<MetaObject> metaObjectValidator)
        {
            _metaObjectValidator = metaObjectValidator;
        }

        public bool Validate(FloorMap record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (record.MapId == 0)
                result.AddError("MapId", "Map ID is required");

            return result.IsValid();
        }


        public bool ValidateNew(FloorMap record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (record.BuildingId == 0)
                result.AddError("BuildingId", "Building ID is required");

            return result.IsValid();
        }

        private bool ValidateCommon(FloorMap record, out IValidationResult result)
        {
            _metaObjectValidator.Validate(record, out result);

            if (record.FloorOrdinal == 0)
                result.AddError("FloorOrdinal", "Floor Ordinal is required");

            if (string.IsNullOrEmpty(record.FloorCode))
                result.AddError("FloorCode", "Floor Code is required");
            else if (record.FloorCode.Length > 4)
                result.AddError("FloorCode", "Floor Code must not exceed 4 characters");

            if (string.IsNullOrEmpty(record.FloorLabel))
                result.AddError("FloorLabel", "Floor Label is required");
            else if (record.FloorLabel.Length > 100)
                result.AddError("FloorLabel", "Floor Label must not exceed 100 characters");

            return result.IsValid();
        }
    }
}