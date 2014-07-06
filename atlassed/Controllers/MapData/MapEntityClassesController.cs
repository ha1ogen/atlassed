using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    [RoutePrefix("api/MapEntityClasses")]
    public class MapEntityClassesController : SinglePageAppApiController
    {
        public IEnumerable<MapEntityClass> Get()
        {
            return MapEntityClass.GetAllMapEntityClasses();
        }

        [Route("{className}")]
        public MapEntityClass Get(string className)
        {
            var mec = MapEntityClass.GetMapEntityClass(className);
            if (mec == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

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

        [Route("{className}")]
        public bool Delete(string className)
        {
            var mec = MapEntityClass.GetMapEntityClass(className);
            if (mec == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return mec.Delete();
        }
    }
}