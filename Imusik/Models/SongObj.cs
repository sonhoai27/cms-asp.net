using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imusik.Models
{
    public class SongObj
    {
        public List<Author> authors { get; set; }
        public List<Kind> kinds { get; set; }
    }
}