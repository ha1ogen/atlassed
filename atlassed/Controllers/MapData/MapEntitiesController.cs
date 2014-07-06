using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    public class MapEntitiesController : SinglePageAppApiController
    {
        [Route("api/maps/{mapId}/entities")]
        public IEnumerable<MapEntity> GetMapEntities(int mapId, string classNames = "")
        {
            if (CampusMap.GetCampus(mapId) == null && FloorMap.GetFloor(mapId) == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return MapEntity.GetAllMapEntities(mapId, classNames);
        }

        [HttpGet]
        public List<SearchResult> Search([FromUri]string q, [FromUri]string classNames = "", int? mapId = null, int skip = 0, int? take = null)
        {
            if (take <= 0) throw new ArgumentOutOfRangeException("take", take, "take must be a positive integer");

            return MapEntity.Search(q, classNames, mapId, skip, take);
        }

        public MapEntity Get(int id)
        {
            var e = MapEntity.GetMapEntity(id);
            if (e == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return e;
        }

        public HttpResponseMessage Post([FromBody]MapEntity entity)
        {
            var me = MapEntity.Create(entity);
            return Request.CreateResponse(HttpStatusCode.Created, me);
        }

        public MapEntity Put([FromBody]MapEntity entity)
        {
            var e = MapEntity.Update(entity);
            if (e == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return e.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var e = MapEntity.GetMapEntity(id);
            if (e == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return e.Delete();
        }
    }
}