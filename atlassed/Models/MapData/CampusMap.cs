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
        public string CampusName { get; set; }
        public Coordinate MapCoordinates { get; set; }

        public CampusMap()
        {
            MapCoordinates = new Coordinate();
        }
    }

    public class CampusMapValidator : IValidator<CampusMap>
    {
        public bool Validate(CampusMap record, out ICollection<ValidationError> errors)
        {
            errors = new List<ValidationError>();
            return true;
        }
    }
}