using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imusik.Models
{
    public class LoveSum
    {
        public double? noLove { get; set; }
        public double? littleLove { get; set; }
        public double? love { get; set; }
        public double? superLove { get; set; }
        public double? lotsofLove { get; set; }

        public LoveSum(double? noLove, double? littleLove, double? love, double? superLove, double? lotsofLove)
        {
            this.noLove = noLove;
            this.littleLove = littleLove;
            this.love = love;
            this.superLove = superLove;
            this.lotsofLove = lotsofLove;
        }
    }
}