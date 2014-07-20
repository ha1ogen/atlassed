using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class Building : MetaObject
    {
        public override int ObjectId { get { return BuildingId; } }
        public override string _className { get { return typeof(Building).Name; } }
        public int BuildingId { get; set; }
        public int CampusMapId { get; set; }
        public string BuildingAddress { get; set; }
    }

    public class NewBuilding : Building
    {
        public List<Coordinate> EntityCoordinates { get; set; }
    }

    public class BuildingValidator : IValidatorWNew<Building, NewBuilding>
    {
        private IValidator<MetaObject> _metaObjectValidator;

        public BuildingValidator(IValidator<MetaObject> metaObjectValidator)
        {
            _metaObjectValidator = metaObjectValidator;
        }

        public bool Validate(Building record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (record.BuildingId == 0)
                result.AddError("BuildingId", "Building ID is required");

            return result.IsValid();
        }

        public bool ValidateNew(NewBuilding record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (record.CampusMapId == 0)
                result.AddError("CampusMapId", "Campus Map ID is required");

            if (record.EntityCoordinates == null || !record.EntityCoordinates.Any())
                result.AddError("EntityCoordinates", "At least one entity coordinate must be provided.");

            return result.IsValid();
        }

        private bool ValidateCommon(Building record, out IValidationResult result)
        {
            _metaObjectValidator.Validate(record, out result);

            if (record.BuildingAddress == null || record.BuildingAddress == string.Empty)
                result.AddError("BuildingAddress", "Building Address is required");
            else if (record.BuildingAddress.Length > 100)
                result.AddError("BuildingAddress", "Building Address cannot exceed 100 characters");

            return result.IsValid();
        }
    }
}