using Atlassed.Models;
using Atlassed.Models.MapData;
using Atlassed.Repositories;
using Atlassed.Repositories.MapData;
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
        private ISearchableRepository<MapEntity, MapEntity, int, int, SearchResult> _repository;
        private IExistenceRepository<int> _mapRepository;

        public MapEntitiesController(SqlConnectionFactory f)
        {
            _repository = new MapEntityRepository(f, new MapEntityValidator(new MetaObjectValidator(new MetaFieldRepository(f))));
            _mapRepository = new MapRepository(f);
        }

        [Route("api/maps/{mapId}/entities")]
        public IEnumerable<MapEntity> GetMapEntities(int mapId, string classNames = "")
        {
            if (!_mapRepository.RecordExists(mapId))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var results = _repository.GetMany(mapId);

            if (classNames != string.Empty)
            {
                var classes = classNames.Split(',');
                results = results.Where(x => classes.Contains(x.ClassName));
            }

            return results;
        }

        [HttpGet]
        public List<SearchResult> Search([FromUri]string q, [FromUri]string classNames = "", [FromUri]int? mapId = null, [FromUri]int skip = 0, [FromUri]int? take = null)
        {
            if (take <= 0) throw new ArgumentOutOfRangeException("take", take, "take must be a positive integer");

            var results = _repository.Search(q, skip, take);

            if (mapId != null)
            {
                results = results.Where(x => x.SecondaryId == mapId);
            }

            if (classNames != string.Empty)
            {
                var classes = classNames.Split(',');
                results = results.Where(x => classes.Contains(x.ClassName));
            }

            return results.ToList();
        }

        public MapEntity Get(int id)
        {
            var e = _repository.GetOne(id);
            if (e == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return e;
        }

        public HttpResponseMessage Post([FromBody]MapEntity entity)
        {
            if (entity == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            var me = _repository.Create(entity, out validationResult);
            if (!validationResult.IsValid())
                return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

            return Request.CreateResponse(HttpStatusCode.Created, me);
        }

        public HttpResponseMessage Put([FromBody]MapEntity entity)
        {
            if (entity == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            if (!_repository.Update(ref entity, out validationResult))
            {
                if (!validationResult.IsValid())
                    return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, entity);
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}