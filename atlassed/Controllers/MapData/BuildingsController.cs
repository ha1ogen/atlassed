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
        private IExistenceRepository<int> _campusRepository;

        public BuildingsController(SqlConnectionFactory f)
        {
            _repository = new BuildingRepository(f, new BuildingValidator(new MetaObjectValidator(new MetaFieldRepository(f))));
            _campusRepository = new CampusRepository(f, new CampusMapValidator(new MetaObjectValidator(new MetaFieldRepository(f))));
        }

        public IEnumerable<Building> Get()
        {
            return _repository.GetMany();
        }

        [Route("api/campuses/{campusId}/buildings")]
        public IEnumerable<Building> GetBuildings(int campusId)
        {
            if (!_campusRepository.RecordExists(campusId))
                throw new HttpResponseException(HttpStatusCode.NotFound);

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
            if (building == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            var b = _repository.Create(building, out validationResult);
            if (!validationResult.IsValid())
                return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

            return Request.CreateResponse(HttpStatusCode.Created, b);
        }

        public HttpResponseMessage Put([FromBody]Building building)
        {
            if (building == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            if (!_repository.Update(ref building, out validationResult))
            {
                if (!validationResult.IsValid())
                    return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(building);
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}