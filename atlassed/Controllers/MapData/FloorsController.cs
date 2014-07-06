using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    public class FloorsController : SinglePageAppApiController
    {
        public FloorMap Get(int id)
        {
            var f = FloorMap.GetFloor(id);
            if (f == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return f;
        }

        public HttpResponseMessage Post([FromBody]FloorMap floor)
        {
            var f = FloorMap.Create(floor);
            return Request.CreateResponse(HttpStatusCode.Created, f);
        }

        public FloorMap Put([FromBody]FloorMap floor)
        {
            var f = FloorMap.Update(floor);
            if (f == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return f.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var f = FloorMap.GetFloor(id);
            if (f == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return f.Delete();
        }
    }
}