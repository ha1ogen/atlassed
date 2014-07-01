using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers
{
    public class FloorsController : ApiController
    {
        // GET api/<controller>/5
        public FloorMap Get(int id)
        {
            return FloorMap.GetFloor(id);
        }

        // POST api/<controller>
        public FloorMap Post([FromBody]FloorMap floor)
        {
            return FloorMap.Create(floor);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}