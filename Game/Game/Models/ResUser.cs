using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class ResUser
    {
        public int status { get; set; }
        public int idUser { get; set; }
        public string message { get; set; }

        public ResUser(int status, int idUser,string message)
        {
            this.status = status;
            this.message = message;
            this.idUser = idUser;
        }
    }
}