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
        public double? lotsofLove { get; set; }
        public double? superLove { get; set; }

        public LoveSum(double? noLove, double? littleLove, double? love, double? lotsofLove, double? superLove)
        {
            this.noLove = noLove;
            this.littleLove = littleLove;
            this.love = love;
            this.lotsofLove = lotsofLove;
            this.superLove = superLove;
        }
    }
}