using Atlassed.Models;
using Atlassed.Models.MapData;
using Atlassed.Repositories;
using Atlassed.Repositories.MapData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Atlassed.Controllers.MapData
{
    public class BuildingsController : SinglePageAppApiController
    {
        private IRepository<Building, NewBuilding, int, int?> _repository;

        public BuildingsController(SqlConnectionFactory f)
        {
            _repository = new BuildingRepository(f, new BuildingValidator());
        }

        public IEnumerable<Building> Get()
        {
            return _repository.GetMany();
        }

        [Route("api/campuses/{campusId}/buildings")]
        public IEnumerable<Building> GetBuildings(int campusId)
        {
            return _repository.GetMany(campusId);
        }

        public Building Get(int id)
        {
            var b = _repository.GetOne(id);
            if (b == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return b;
        }

        public HttpResponseMessage Post([FromBody]NewBuilding building)
        {
            ICollection<ValidationError> errors;
            var b = _repository.Create(building, out errors);
            if (b == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            return Request.CreateResponse(HttpStatusCode.Created, b);
        }

        public Building Put([FromBody]Building building)
        {
            ICollection<ValidationError> errors;
            if (!_repository.Update(ref building, out errors))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return building;
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}