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
        private IExistenceRepository<int> _buildingRepository;

        public FloorsController(SqlConnectionFactory f)
        {
            _repository = new FloorRepository(f, new FloorMapValidator(new MetaObjectValidator(new MetaFieldRepository(f))));
            _buildingRepository = new BuildingRepository(f, new BuildingValidator(new MetaObjectValidator(new MetaFieldRepository(f))));
        }

        [Route("api/buildings/{id}/floors")]
        public IEnumerable<FloorMap> GetFloors(int id)
        {
            if (!_buildingRepository.RecordExists(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return _repository.GetMany(id);
        }

        public FloorMap Get(int id)
        {
            var f = _repository.GetOne(id);
            if (f == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return f;
        }

        [Route("api/buildings/{buildingId}/floors")]
        public HttpResponseMessage PostMany(int buildingId, [FromBody]IEnumerable<FloorMap> floors)
        {
            if (floors == null || !floors.Any())
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please provide at least one floor");

            if (!_buildingRepository.RecordExists(buildingId))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid building ID");

            var createdFloors = new List<FloorMap>();
            var validationResults = new List<IValidationResult>();

            foreach (var floor in floors)
            {
                if (floor == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "null floor provided");

                if (floor.BuildingId != 0)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "buildingId should not be provided with each floor, it is provided in the request instead");

                floor.BuildingId = buildingId;

                IValidationResult validationResult;
                createdFloors.Add(_repository.Create(floor, out validationResult));
                validationResults.Add(validationResult);
            }

            if (validationResults.Where(x => !x.IsValid()).Any())
                return Request.CreateResponse(HttpStatusCode.BadRequest, validationResults);

            return Request.CreateResponse(HttpStatusCode.Created, createdFloors);
        }

        public HttpResponseMessage Post([FromBody]FloorMap floor)
        {
            if (floor == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            var f = _repository.Create(floor, out validationResult);
            if (!validationResult.IsValid())
                return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

            return Request.CreateResponse(HttpStatusCode.Created, f);
        }

        public HttpResponseMessage Put([FromBody]FloorMap floor)
        {
            if (floor == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            if (!_repository.Update(ref floor, out validationResult))
            {
                if (!validationResult.IsValid())
                    return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, floor);
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}