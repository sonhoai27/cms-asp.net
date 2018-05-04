using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Newtonsoft.Json.Linq;
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
                                 });
                    var yeu = (from l in db.Loves
                               join u in db.Users on l.idUser equals u.idUser
                               orderby l.idLove descending
                               where l.idSong == id
                               select new
                               {
                                   noLove = l.noLove,
                                   littleLove = l.littleLove,
                                   love = l.love1,
                                   lotsofLove = l.lotsofLove,
                                   superLove = l.superLove

                               });

                    LoveSum sum = new LoveSum(
                            (int)yeu.Sum(e => e.noLove),
                            (int)yeu.Sum(e => e.littleLove),
                            (int)yeu.Sum(e => e.love),
                            (int)yeu.Sum(e => e.lotsofLove),
                            (int)yeu.Sum(e => e.superLove)
                        );

                    var user = (from l in db.Loves
                               where l.idSong == id && l.idUser == idUser
                               select new
                               {
                                   idLove = l.idLove,
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
        public IHttpActionResult GetLoves(int idSong)
        {
            try
            {
                var yeu = (from l in db.Loves
                           join u in db.Users on l.idUser equals u.idUser
                           orderby l.idLove descending
                           where l.idSong == idSong
                           select new
                           {
                               noLove = l.noLove,
                               littleLove = l.littleLove,
                               love = l.love1,
                               lotsofLove = l.lotsofLove,
                               superLove = l.superLove

                           });

                LoveSum sum = new LoveSum(
                        (int)yeu.Sum(e => e.noLove),
                        (int)yeu.Sum(e => e.littleLove),
                        (int)yeu.Sum(e => e.love),
                        (int)yeu.Sum(e => e.lotsofLove),
                        (int)yeu.Sum(e => e.superLove)
                    );


                return Json(
                        sum
                    );
            }catch(Exception e)
            {

            }

            return null;
        }
        // PUT: api/Loves/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLove(int id, Love love, Int32 idUser, string token)
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
                    if (!ModelState.IsValid)
                    {
                        return Json(new Models.TokenUser(
                                   400,
                                    "error",
                                    0
                                   ));
                    }

                    if (id != love.idLove)
                    {
                        return Json(new Models.TokenUser(
                                  400,
                                   "error",
                                   0
                                  ));
                    }

                    db.Entry(love).State = EntityState.Modified;

                    try
                    {
                        db.SaveChanges();
                        return Json(new Models.TokenUser(
                                  200,
                                   "ok",
                                   0
                                  ));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LoveExists(id))
                        {
                            return Json(new Models.TokenUser(
                                  400,
                                   "error",
                                   0
                                  ));
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new Models.TokenUser(
                                  400,
                                   "error",
                                   0
                                  ));
            }
            return Json(new Models.TokenUser(
                                  400,
                                   "error",
                                   0
                                  ));
        }

        // POST: api/Loves
        [ResponseType(typeof(Love))]
        public IHttpActionResult PostLove(Love love, Int32 idUser, string token)
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
                    if (!ModelState.IsValid)
                    {
                        return Json(new Models.TokenUser(
                                  400,
                                   "error",
                                   0
                                  ));
                    }

                    db.Loves.Add(love);
                    db.SaveChanges();

                    return Json(new Models.TokenUser(
                                  200,
                                   "ok",
                                   0
                                  ));
                }
            }catch(Exception e)
            {
                return Json(new Models.TokenUser(
                                  400,
                                   "error",
                                   0
                                  ));
            }
            return Json(new Models.TokenUser(
                                  400,
                                   "error",
                                   0
                                  ));
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