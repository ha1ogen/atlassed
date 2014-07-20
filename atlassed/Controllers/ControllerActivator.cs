using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Atlassed.Controllers
{
    public class ControllerActivator : IHttpControllerActivator
    {
        public ControllerActivator(HttpConfiguration configuration) { }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            if (controllerType.GetConstructor(new Type[] { typeof(SqlConnectionFactory) }) != null)
            {
                return (IHttpController)Activator.CreateInstance(controllerType, new SqlConnectionFactory());
            }

            return (IHttpController)Activator.CreateInstance(controllerType);
        }
    }
}