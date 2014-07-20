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
    public class BusinessRulesController : SinglePageAppApiController
    {
        private IRepository<BusinessRule, BusinessRule, int, int> _repository;
        private IExistenceRepository<int> _businessRuleClassRepository;

        public BusinessRulesController(SqlConnectionFactory f)
        {
            _repository = new BusinessRuleRepository(f, new BusinessRuleValidator(new MetaObjectValidator(new MetaFieldRepository(f))));
            _businessRuleClassRepository = new BusinessRuleClassRepository(f, new BusinessRuleClassValidator(new MetaClassValidator()));
        }

        [Route("api/BusinessRuleClasses/{classId}/rules")]
        public IEnumerable<BusinessRule> Get(int classId)
        {
            if (!_businessRuleClassRepository.RecordExists(classId))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return _repository.GetMany(classId);
        }

        public BusinessRule GetBusinessRule(int id)
        {
            var br = _repository.GetOne(id);
            if (br == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return br;
        }

        public HttpResponseMessage Post([FromBody]BusinessRule businessRule)
        {
            if (businessRule == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            var br = _repository.Create(businessRule, out validationResult);
            if (!validationResult.IsValid())
                return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

            return Request.CreateResponse(HttpStatusCode.Created, br);
        }

        public HttpResponseMessage Put([FromBody]BusinessRule businessRule)
        {
            if (businessRule == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            if (!_repository.Update(ref businessRule, out validationResult))
            {
                if (!validationResult.IsValid())
                    return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, businessRule);
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}