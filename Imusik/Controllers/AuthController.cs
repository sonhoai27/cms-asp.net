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
                User user = imusik.Users.Where(u => u.pass == client.pass && u.email == client.email).First();
                if (user.email.Equals(client.email) && user.pass.Equals(client.pass))
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
                            200,
                             "error",
                             0
                            ));
                }
            }catch(Exception e)
            {
                return Json(new TokenUser(
                           200,
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
                               200,
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
                              200,
                               "error",
                               0
                              ));
            }

            return Json(new TokenUser(
                              200,
                               "error",
                               0
                              ));
        }
    }
}
