using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlassed.Models.UserManagement
{
    public class Session
    {
        public Guid SessionId { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string> Roles { get; set; }

        public Session(Credentials credentials)
        {
            SessionId = new Guid();
        }
        public bool Destroy()
        {
            return false;
        }

        public static Session GetSession(Guid sessionId)
        {
            return null;
        }
    }
}