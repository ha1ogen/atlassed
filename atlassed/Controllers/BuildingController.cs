using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Atlassed.Controllers
{
    [RoutePrefix("api/buildings")]
    public class BuildingsController : ApiController
    {
        [Route("{buildingId:int}/floors")]
        public List<FloorMap> GetFloors(int buildingId)
        {
            if (Building.GetBuilding(buildingId) == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return FloorMap.GetAllFloors(buildingId);
        }

        public Building Get(int id)
        {
            var b = Building.GetBuilding(id);
            if (b == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return b;
        }

        public Building Post([FromBody]NewBuilding building)
        {
            return Building.Create(building, building.EntityPoints);
        }

        public Building Put([FromBody]Building building)
        {
            var b = Building.Update(building);
            if (b == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return b.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var b = Building.GetBuilding(id);
            if (b == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return b.Delete();
        }
    }

    public class NewBuilding : Building
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EntityPoints { get; set; }
    }
}