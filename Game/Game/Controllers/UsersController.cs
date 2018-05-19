using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Game;
using Game.Models;
using JWT.Algorithms;
using JWT.Builder;

namespace Game.Controllers
{
    public class UsersController : ApiController
    {
        const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        private NotesEntities db = new NotesEntities();

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(int id, string token)
        {
            if (checkToken(token))
            {
                User user = await db.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            return Ok("ERROR");
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id, string token, User user)
        {
            if (checkToken(token))
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(user.pass));

                StringBuilder sbHash = new StringBuilder();

                foreach (byte b in bHash)
                {

                    sbHash.Append(String.Format("{0:x2}", b));

                }
                string passUser = sbHash.ToString();

                if (!ModelState.IsValid)
                {
                    return Ok("ERROR");
                }

                if (id != user.iduser)
                {
                    return Ok("ERROR");
                }

                user.pass = passUser;

                db.Entry(user).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                    return Ok("OK");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return Ok("ERROR");
                    }
                    else
                    {
                        throw;
                    }
                }
            }else
            {
                return Ok("ERROR");
            }
           
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser([FromBody]User user)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(user.pass));

            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {

                sbHash.Append(String.Format("{0:x2}", b));

            }
            user.pass = sbHash.ToString();
            db.Users.Add(user);
            db.SaveChanges();
            var token = new JwtBuilder()
                               .WithAlgorithm(new HMACSHA256Algorithm())
                               .WithSecret(secret)
                               .AddClaim("exp", DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds())
                               .AddClaim("idUser", user.iduser)
                       .Build();
            return Json(new ResUser(
                    200,
                    user.iduser,
                    token
                ));
        }

        [Route("api/Users/Login/")]//custom dang filter and sort
        [HttpPost]
        public IHttpActionResult PostLogin([FromBody]User client)
        {

            try { 
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(client.pass));

                StringBuilder sbHash = new StringBuilder();

                foreach (byte b in bHash)
                {

                    sbHash.Append(String.Format("{0:x2}", b));

                }
                string passUser = sbHash.ToString();
                User user = db.Users.Where(u => u.pass == passUser && u.email == client.email).First();
                if (user.email.Equals(client.email) && user.pass.Equals(passUser))
                {
                    var token = new JwtBuilder()
                                .WithAlgorithm(new HMACSHA256Algorithm())
                                .WithSecret(secret)
                                .AddClaim("exp", DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds())
                                .AddClaim("idUser", user.iduser)
                                .Build();
                    ResUser userToken = new ResUser(
                        200,
                        user.iduser,
                         token
                        );
                    return Json(userToken);
                }
                else
                {
                    return Json(new ResUser(
                            400,
                             0,
                             "error"
                            ));
                }
            }
            catch (Exception e)
            {
                return Json(new ResUser(
                           400,
                           0,
                            "error"
                           ));
            }
        }


        [Route("api/Users/Check/")]//custom dang filter and sort
        [HttpPost]
        public IHttpActionResult PostCheck(string token)
        {
            Newtonsoft.Json.Linq.JObject jsonObj = null;
            try
            {
                string json = new JwtBuilder()
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token);

                jsonObj = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (jsonObj == null)
                {
                    return Json(new ResUser(
                               400,
                               0,
                                "error"
                               ));
                }
                else if (jsonObj != null)
                {
                    return Json(new ResUser(
                               200,
                               0,
                                "ok"
                               ));
                }
            }
            catch (Exception e)
            {
                return Json(new ResUser(
                              400,
                              0,
                               "error"
                              ));
            }

            return Json(new ResUser(
                              400,
                              0,
                               "error"
                              ));
        }


        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.iduser == id) > 0;
        }

        private bool checkToken(string token)
        {
            Newtonsoft.Json.Linq.JObject jsonObj = null;
            try
            {
                string json = new JwtBuilder()
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token);

                jsonObj = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (jsonObj == null)
                {
                    return false;
                }
                else if (jsonObj != null)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return false;
        }
    }
}