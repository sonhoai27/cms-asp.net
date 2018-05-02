using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imusik.Models
{
    public class LoveUser
    {
        public int love { get; set; }
        public int noLove { get; set; }
        public int littleLove { get; set; }
        public int superLove { get; set; }
        public int lotsofLove { get; set; }
        public int idUser { get; set; }
        public LoveUser(int user, int love, int littleLove, int superLove, int lotsofLove, int noLove)
        {
            this.idUser = user;
            this.love = love;
            this.noLove = noLove;
            this.littleLove = littleLove;
            this.superLove = superLove;
            this.lotsofLove = lotsofLove;
        }
    }
}