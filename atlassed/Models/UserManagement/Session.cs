using System;
using System.Collections.Generic;
using System.Data;
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

            //TODO: DB sessions
            //TODO: authenticate
            //TODO: get roles
        }

        public Session(IDataRecord data)
        {

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