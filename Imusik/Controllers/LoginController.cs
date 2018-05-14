using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Imusik.Controllers
{
    public class LoginController : Controller
    {
        private IMUSIKEntities imusik = new IMUSIKEntities();
        // GET: Login
        public ActionResult Index()
        {
            if (Request.Cookies["idUser"] == null)
            {
                return View();
            }
            else
            {
                return Redirect("http://192.168.10.21:8081");
            }

        }
        [HttpPost]
        public void Check()
        {
            String email = Request["email"];
            String pass = Request["pass"];
            //var a = imusik.Users.Where(u => u.pass == pass).Select(u => new { u.pass, u.email });
            //var email = a.ToList().ElementAt(0).email;
            //var pass = a.ToList().ElementAt(0).email;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));

            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {

                sbHash.Append(String.Format("{0:x2}", b));

            }
            string passUser = sbHash.ToString();
            User user = imusik.Users.Where(u => u.pass == passUser && u.email == email).First();
            if(user.email.Equals(email) && user.pass.Equals(passUser))
            {
                HttpCookie ck = new HttpCookie("idUser");
                ck.Value = user.idUser.ToString();
                ck.Expires = DateTime.Now.AddMinutes(150);
                Response.Cookies.Add(ck);
                if(Request.Cookies["idUser"] != null)
                {
                   Response.Redirect("http://192.168.10.21:8081");
                }
            }
        }
    }
};