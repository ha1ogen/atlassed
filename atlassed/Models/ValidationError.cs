using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlassed.Models
{
    public class ValidationError
    {
        public string Message { get; set; }
        public int Code { get; set; }

        public ValidationError(string message = "", int code = 0)
        {
            Message = message;
            Code = code;
        }
    }
}