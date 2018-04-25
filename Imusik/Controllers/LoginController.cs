using System;
using System.Collections.Generic;
using System.Linq;
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
                return Redirect("http://192.168.137.1:8081");
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
            User user = imusik.Users.Where(u => u.pass == pass && u.email == email).First();
            if(user.email.Equals(email) && user.pass.Equals(pass))
            {
                HttpCookie ck = new HttpCookie("idUser");
                ck.Value = user.idUser.ToString();
                ck.Expires = DateTime.Now.AddMinutes(150);
                Response.Cookies.Add(ck);
                if(Request.Cookies["idUser"] != null)
                {
                   Response.Redirect("http://192.168.137.1:8081");
                }
            }
        }
    }
};