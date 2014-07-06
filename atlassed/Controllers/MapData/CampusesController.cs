using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    public class CampusesController : SinglePageAppApiController
    {
        public IEnumerable<CampusMap> Get()
        {
            return CampusMap.GetAllCampuses();
        }

        public CampusMap Get(int id)
        {
            var c = CampusMap.GetCampus(id);
            if (c == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return c;
        }

        public HttpResponseMessage Post([FromBody]CampusMap campus)
        {
            var c = CampusMap.Create(campus);
            return Request.CreateResponse(HttpStatusCode.Created, c);
        }

        public CampusMap Put([FromBody]CampusMap campus)
        {
            var c = CampusMap.Update(campus);
            if (c == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return c.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var c = CampusMap.GetCampus(id);
            if (c == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return c.Delete();
        }
    }
}