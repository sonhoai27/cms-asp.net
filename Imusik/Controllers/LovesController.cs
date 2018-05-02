using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using Imusik;
using Imusik.Models;
using JWT;
using JWT.Builder;
using Newtonsoft.Json.Linq;

namespace Imusik.Controllers
{
    public class LovesController : ApiController
    {
        const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        private IMUSIKEntities db = new IMUSIKEntities();

        // GET: api/Loves/5
        [ResponseType(typeof(Love))]
        public IHttpActionResult GetLove(int id, Int32 idUser, string token)
        {
            JObject jsonObj = null;
            try
            {
                string json = new JwtBuilder()
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token);

                jsonObj = JObject.Parse(json);

                if (jsonObj.GetValue("idUser").ToString().Equals(idUser + ""))
                {
                    
                    //lấy ds toàn cục theo song
                    var loves = (from l in db.Loves
                                 join u in db.Users on l.idUser equals u.idUser
                                 orderby l.idLove descending
                                 where l.idSong == id
                                 select new
                                 {
                                     idSong = l.idSong,
                                     idUser = u.idUser,
                                     user = u.email,
                                     noLove = l.noLove,
                                     littleLove = l.littleLove,
                                     love = l.love1,
                                     superLove = l.superLove,
                                     lotsofLove = l.lotsofLove
                                 }).Take(10);
                    var noLove = loves.Average(e => e.noLove);
                    var littleLove = loves.Average(e => e.littleLove);
                    var love = loves.Average(e => e.love);
                    var superLove = loves.Average(e => e.superLove);
                    var lotsofLove = loves.Average(e => e.lotsofLove);

                    LoveSum sum = new LoveSum(
                            (int)noLove,
                            (int)littleLove,
                            (int)love,
                            (int)lotsofLove,
                            (int)superLove
                        );

                    var user = (from l in db.Loves
                               where l.idSong == id && l.idUser == idUser
                               select new
                               {
                                   idUser = l.idUser,
                                   idSong = l.idSong,
                                   noLove = l.noLove,
                                   littleLove = l.littleLove,
                                   love = l.love1,
                                   superLove = l.superLove,
                                   lotsofLove = l.lotsofLove
                               }).First();
                    return  Json(
                        new LoveReport(
                            user,
                            loves,
                            sum
                        ));
                }


            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }
            return null;
        }

        // PUT: api/Loves/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLove(int id, Love love)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != love.idLove)
            {
                return BadRequest();
            }

            db.Entry(love).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoveExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Loves
        [ResponseType(typeof(Love))]
        public async Task<IHttpActionResult> PostLove(Love love)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Loves.Add(love);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = love.idLove }, love);
        }

        // DELETE: api/Loves/5
        [ResponseType(typeof(Love))]
        public async Task<IHttpActionResult> DeleteLove(int id)
        {
            Love love = await db.Loves.FindAsync(id);
            if (love == null)
            {
                return NotFound();
            }

            db.Loves.Remove(love);
            await db.SaveChangesAsync();

            return Ok(love);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LoveExists(int id)
        {
            return db.Loves.Count(e => e.idLove == id) > 0;
        }
    }
}