using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Atlassed.Controllers
{
    public class SecuredImageHandler : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            var value = context.Request.Headers.GetValues(SinglePageAppApiController._sessionIdHeader);
            Guid sessionId;
            if (value == null || value.Length != 1 || !Guid.TryParse(value.First(), out sessionId))
            {
                context.Response.StatusCode = 400;
                context.Response.StatusDescription = "Missing/invalid header: Session-Id";
                return;
            }
            // TODO: authorize...

            context.Response.ContentType = "image/png";
            context.Response.WriteFile(context.Request.PhysicalPath);
        }
    }
}