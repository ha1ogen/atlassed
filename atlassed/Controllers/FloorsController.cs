using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers
{
    public class FloorsController : ApiController
    {
        public FloorMap Get(int id)
        {
            var f = FloorMap.GetFloor(id);
            if (f == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return f;
        }

        public FloorMap Post([FromBody]FloorMap floor)
        {
            return FloorMap.Create(floor);
        }

        public FloorMap Put([FromBody]FloorMap floor)
        {
            var f = FloorMap.Update(floor);
            if (f == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return f.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var f = FloorMap.GetFloor(id);
            if (f == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return f.Delete();
        }
    }
}