using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    public class MapEntityClassesController : SinglePageAppApiController
    {
        public IEnumerable<MapEntityClass> Get()
        {
            return MapEntityClass.GetAllMapEntityClasses();
        }

        public MapEntityClass Get(int id)
        {
            var mec = MapEntityClass.GetMapEntityClass(id);
            if (mec == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return mec;
        }

        public HttpResponseMessage Post([FromBody]MapEntityClass mapEntityClass)
        {
            var mec = MapEntityClass.Create(mapEntityClass);
            return Request.CreateResponse(HttpStatusCode.Created, mec);
        }

        public MapEntityClass Put([FromBody]MapEntityClass MapEntityClass)
        {
            var mec = MapEntityClass.Update(MapEntityClass);
            if (mec == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return mec.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var mec = MapEntityClass.GetMapEntityClass(id);
            if (mec == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return mec.Delete();
        }
    }
}