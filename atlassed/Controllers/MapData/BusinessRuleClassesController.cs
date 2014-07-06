using Atlassed.Models.MapData;
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
        public IEnumerable<BusinessRuleClass> Get()
        {
            return BusinessRuleClass.GetAllBusinessRuleClasses();
        }

        [Route("api/BusinessRuleClasses/{className}")]
        public BusinessRuleClass Get(string className)
        {
            var brc = BusinessRuleClass.GetBusinessRuleClass(className);
            if (brc == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return brc;
        }

        public HttpResponseMessage Post([FromBody]BusinessRuleClass businessRuleClass)
        {
            var brc = BusinessRuleClass.Create(businessRuleClass);
            return Request.CreateResponse(HttpStatusCode.Created, brc);
        }

        public BusinessRuleClass Put([FromBody]BusinessRuleClass businessRuleClass)
        {
            var brc = BusinessRuleClass.Update(businessRuleClass);
            if (brc == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return brc.CommitUpdate();
        }

        [Route("api/BusinessRuleClasses/{className}")]
        public bool Delete(string className)
        {
            var brc = BusinessRuleClass.GetBusinessRuleClass(className);
            if (brc == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return brc.Delete();
        }
    }
}