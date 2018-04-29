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

namespace Imusik.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost]
        public string Token()
        {
            return JwtManager.GenerateToken("AAAA");
        }
    }
}
