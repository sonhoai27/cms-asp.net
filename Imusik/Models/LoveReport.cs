using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imusik.Models
{
    public class LoveReport
    {
        public object user;
        public IQueryable<object> loves;
        public LoveSum sum;

        public LoveReport(object user, IQueryable<object> loves, LoveSum sum)
        {
            this.user = user;
            this.loves = loves;
            this.sum = sum;
        }
    }
}