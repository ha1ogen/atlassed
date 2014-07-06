using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Atlassed.Controllers.MapData
{
    public class BuildingsController : SinglePageAppApiController
    {
        [Route("api/buildings/{id}/floors")]
        public List<FloorMap> GetFloors(int id)
        {
            if (Building.GetBuilding(id) == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return FloorMap.GetAllFloors(id);
        }

        public Building Get(int id)
        {
            var b = Building.GetBuilding(id);
            if (b == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return b;
        }

        public HttpResponseMessage Post([FromBody]NewBuilding building)
        {
            var b = Building.Create(building, building.EntityPoints);
            return Request.CreateResponse(HttpStatusCode.Created, b);
        }

        public Building Put([FromBody]Building building)
        {
            var b = Building.Update(building);
            if (b == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return b.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var b = Building.GetBuilding(id);
            if (b == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return b.Delete();
        }
    }

    public class NewBuilding : Building
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<Point> EntityPoints { get; set; }
    }
}