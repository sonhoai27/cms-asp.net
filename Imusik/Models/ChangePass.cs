using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imusik.Models
{
    public class ChangePass
    {
        public int id { get; set; }
        public string pass { get; set; }
        public ChangePass(int id, string pass)
        {
            this.id = id;
            this.pass = pass;
        }
    }
}