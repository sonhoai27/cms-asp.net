using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imusik.Models
{
    public class TokenUser
    {
        public string message { get; set; }
        public int status { get; set; }
        public int id { get; set; }
        public TokenUser(int status,string message, int id)
        {
            this.message = message;
            this.status = status;
            this.id = id;
        }
    }
}