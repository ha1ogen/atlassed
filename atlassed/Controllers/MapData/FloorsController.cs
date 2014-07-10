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
    public class FloorsController : SinglePageAppApiController
    {
        private IRepository<FloorMap, FloorMap, int, int> _repository;
        private IRepository<Building, NewBuilding, int, int?> _buildingRepository;

        public FloorsController(SqlConnectionFactory f)
        {
            _repository = new FloorRepository(f, new FloorMapValidator());
            _buildingRepository = new BuildingRepository(f, new BuildingValidator());
        }

        [Route("api/buildings/{id}/floors")]
        public IEnumerable<FloorMap> GetFloors(int id)
        {
            if (_buildingRepository.GetOne(id) == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return _repository.GetMany(id);
        }

        public FloorMap Get(int id)
        {
            var f = _repository.GetOne(id);
            if (f == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return f;
        }

        public HttpResponseMessage Post([FromBody]FloorMap floor)
        {
            IEnumerable<ValidationError> errors;
            var f = _repository.Create(floor, out errors);
            return Request.CreateResponse(HttpStatusCode.Created, f);
        }

        public FloorMap Put([FromBody]FloorMap floor)
        {
            IEnumerable<ValidationError> errors;
            if (!_repository.Update(ref floor, out errors))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return floor;
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}