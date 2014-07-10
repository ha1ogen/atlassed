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
        public int BuildingId { get; set; }
        public int FloorOrdinal { get; set; }
        public string FloorCode { get; set; }
        public string FloorLabel { get; set; }
    }

    public class FloorMapValidator : IValidator<FloorMap>
    {
        public bool Validate(FloorMap record, out ICollection<ValidationError> errors)
        {
            errors = new List<ValidationError>();
            return true;
        }
    }
}