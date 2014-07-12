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
            _repository = new BusinessRuleRepository(f, new BusinessRuleValidator());
            _businessRuleClassRepository = new BusinessRuleClassRepository(f, new BusinessRuleClassValidator());
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
            ICollection<ValidationError> errors;
            var br = _repository.Create(businessRule, out errors);
            return Request.CreateResponse(HttpStatusCode.Created, br);
        }

        public BusinessRule Put([FromBody]BusinessRule businessRule)
        {
            ICollection<ValidationError> errors;
            if (!_repository.Update(ref businessRule, out errors))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return businessRule;
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}