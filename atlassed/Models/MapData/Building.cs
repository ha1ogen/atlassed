using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class Building : MetaObject
    {
        public int BuildingId { get; set; }
        public int CampusMapId { get; set; }
        public string BuildingAddress { get; set; }
    }

    public class NewBuilding : Building
    {
        public List<Coordinate> EntityCoordinates { get; set; }
    }

    public class BuildingValidator : IValidator<Building>
    {
        public bool Validate(Building record, out ICollection<ValidationError> errors)
        {
            errors = new List<ValidationError>();

            if (record.BuildingAddress.Length > 100)
            {
                errors.Add(new ValidationError("BuildingAddress cannot exceed 100 characters"));
            }

            return !errors.Any();
        }
    }
}