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
        private IRepository<CampusMap, CampusMap, int, int?> _campusRepository;
        private IRepository<FloorMap, FloorMap, int, int> _floorRepository;

        public MapEntitiesController(SqlConnectionFactory f)
        {
            _repository = new MapEntityRepository(f, new MapEntityValidator());
            _campusRepository = new CampusRepository(f, new CampusMapValidator());
            _floorRepository = new FloorRepository(f, new FloorMapValidator());
        }

        [Route("api/maps/{mapId}/entities")]
        public IEnumerable<MapEntity> GetMapEntities(int mapId, string classNames = "")
        {
            if (_campusRepository.GetOne(mapId) == null && _floorRepository.GetOne(mapId) == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var classes = classNames.Split(',');
            return _repository.GetMany(mapId)
                .Where(x => classes.Contains(x.ClassName));
        }

        [HttpGet]
        public List<SearchResult> Search([FromUri]string q, [FromUri]string classNames = "", int? mapId = null, int skip = 0, int? take = null)
        {
            if (take <= 0) throw new ArgumentOutOfRangeException("take", take, "take must be a positive integer");

            IEnumerable<SearchResult> results = _repository.Search(q, skip, take);

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
            IEnumerable<ValidationError> errors;
            var me = _repository.Create(entity, out errors);
            return Request.CreateResponse(HttpStatusCode.Created, me);
        }

        public MapEntity Put([FromBody]MapEntity entity)
        {
            IEnumerable<ValidationError> errors;
            if (!_repository.Update(ref entity, out errors))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return entity;
        }

        public void Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}