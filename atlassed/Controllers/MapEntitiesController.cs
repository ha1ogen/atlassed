using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers
{
    public class MapEntitiesController : ApiController
    {
        [Route("api/maps/{mapId}/entities")]
        public IEnumerable<MapEntity> GetMapEntities(int mapId, string classNames = "")
        {
            if (CampusMap.GetCampus(mapId) == null && FloorMap.GetFloor(mapId) == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return MapEntity.GetAllMapEntities(mapId, classNames);
        }

        public MapEntity Get(int id)
        {
            var e = MapEntity.GetMapEntity(id);
            if (e == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return e;
        }

        public MapEntity Post([FromBody]MapEntity entity)
        {
            return MapEntity.Create(entity);
        }

        public MapEntity Put([FromBody]MapEntity entity)
        {
            var e = MapEntity.Update(entity);
            if (e == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return e.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var e = MapEntity.GetMapEntity(id);
            if (e == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return e.Delete();
        }
    }
}