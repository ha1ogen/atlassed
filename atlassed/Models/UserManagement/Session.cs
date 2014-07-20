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
        public string Username { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastPing { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string> Roles { get; set; }
    }
}