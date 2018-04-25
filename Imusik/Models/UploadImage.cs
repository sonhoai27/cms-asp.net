using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Imusik.Models
{
    public  class UploadImage
    {
        public static string uploadImage(HttpPostedFileBase file)
        {
            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Cover"),
                Path.GetFileName(file.FileName));
            file.SaveAs(path);
            return "/Cover/"+file.FileName;
        }
    }
}