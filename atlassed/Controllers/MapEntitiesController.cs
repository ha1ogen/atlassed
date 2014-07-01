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
            return MapEntity.GetAllMapEntities(mapId, classNames);
        }

        // GET api/<controller>/5
        public MapEntity Get(int id)
        {
            return MapEntity.GetMapEntity(id);
        }

        // POST api/<controller>
        public MapEntity Post([FromBody]MapEntity entity)
        {
            return MapEntity.Create(entity);
        }

        // PUT api/<controller>/5
        public MapEntity Put([FromBody]MapEntity value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/<controller>/5
        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}