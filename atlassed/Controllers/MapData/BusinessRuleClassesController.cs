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
    public class BusinessRuleClassesController : SinglePageAppApiController
    {
        private IRepository<BusinessRuleClass, BusinessRuleClass, int, int?> _repository;

        public BusinessRuleClassesController(SqlConnectionFactory f)
        {
            _repository = new BusinessRuleClassRepository(f, new BusinessRuleClassValidator());
        }

        public IEnumerable<BusinessRuleClass> Get()
        {
            return _repository.GetMany();
        }

        public BusinessRuleClass Get(int id)
        {
            var brc = _repository.GetOne(id);
            if (brc == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return brc;
        }

        public HttpResponseMessage Post([FromBody]BusinessRuleClass businessRuleClass)
        {
            ICollection<ValidationError> errors;
            var brc = _repository.Create(businessRuleClass, out errors);
            return Request.CreateResponse(HttpStatusCode.Created, brc);
        }

        public BusinessRuleClass Put([FromBody]BusinessRuleClass businessRuleClass)
        {
            ICollection<ValidationError> errors;
            if (!_repository.Update(ref businessRuleClass, out errors))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return businessRuleClass;
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}