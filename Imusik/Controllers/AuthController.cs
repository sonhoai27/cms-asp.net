using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using Imusik.Models;
using JWT.Builder;
using JWT.Algorithms;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Data.Entity;

namespace Imusik.Controllers
{
    public class AuthController : ApiController
    {
        const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        private IMUSIKEntities imusik = new IMUSIKEntities();

        [HttpPost]
        public IHttpActionResult Login([FromBody]User client)
        {

            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(client.pass));

                StringBuilder sbHash = new StringBuilder();

                foreach (byte b in bHash)
                {

                    sbHash.Append(String.Format("{0:x2}", b));

                }
                string passUser = sbHash.ToString();
                User user = imusik.Users.Where(u => u.pass == passUser && u.email == client.email).First();
                if (user.email.Equals(client.email) && user.pass.Equals(passUser))
                {
                    var token = new JwtBuilder()
                                .WithAlgorithm(new HMACSHA256Algorithm())
                                .WithSecret(secret)
                                .AddClaim("exp", DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds())
                                .AddClaim("idUser", user.idUser)
                                .Build();
                    TokenUser userToken = new TokenUser(
                        200,
                         token,
                         user.idUser
                        );
                    return Json(userToken);
                }
                else
                {
                    return Json(new TokenUser(
                            400,
                             "error",
                             0
                            ));
                }
            }catch(Exception e)
            {
                return Json(new TokenUser(
                           400,
                            "error",
                            0
                           ));
            }
        }

        [HttpGet]
        public IHttpActionResult CheckToken(string token)
        {
            JObject jsonObj = null;
            try
            {
                string json = new JwtBuilder()
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token);

                jsonObj = JObject.Parse(json);
                if (jsonObj == null)
                {
                    return Json(new TokenUser(
                               400,
                                "error",
                                0
                               ));
                }else if(jsonObj != null)
                {
                    return Json(new TokenUser(
                               200,
                                "ok",
                                0
                               ));
                }
            }catch(Exception e)
            {
                return Json(new TokenUser(
                              400,
                               "error",
                               0
                              ));
            }

            return Json(new TokenUser(
                              400,
                               "error",
                               0
                              ));
        }

        [Route("api/Auth/Register/")]//custom dang filter and sort
        [HttpPost]
        public IHttpActionResult PostRegister([FromBody]User user)
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
            imusik.Users.Add(user);
            imusik.SaveChanges();
            var token = new JwtBuilder()
                               .WithAlgorithm(new HMACSHA256Algorithm())
                               .WithSecret(secret)
                               .AddClaim("exp", DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds())
                               .AddClaim("idUser", user.idUser)
                       .Build();
            return Json(new TokenUser(
                    200,
                    token,
                    user.idUser
                ));
        }
        [Route("api/Auth/Changepass/")]//custom dang filter and sort
        [HttpPost]
        public IHttpActionResult PostChangepass([FromBody]ChangePass changePass, string token)
        {
            if (checkToken(token))
            {
                User user = imusik.Users.Find(changePass.id);
                if (user.idUser == changePass.id && user!=null)
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(changePass.pass));

                    StringBuilder sbHash = new StringBuilder();

                    foreach (byte b in bHash)
                    {

                        sbHash.Append(String.Format("{0:x2}", b));

                    }
                    user.pass = sbHash.ToString();
                    imusik.Entry(user).State = EntityState.Modified;
                    imusik.SaveChanges();

                    return Ok("200");
                }else
                {
                    return Ok("400");
                }
            }

            return Ok("400");
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
