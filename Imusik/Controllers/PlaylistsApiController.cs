using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using Imusik;
using JWT;
using JWT.Builder;
using Newtonsoft.Json.Linq;

namespace Imusik.Controllers
{
    public class PlaylistsApiController : ApiController
    {
        const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        private IMUSIKEntities db = new IMUSIKEntities();

        // GET: api/PlaylistsApi
        public IEnumerable<Object> GetPlaylists(Int32 list, string token)
        {
            JObject jsonObj = null;
            try
            {
                string json = new JwtBuilder()
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token);

                jsonObj = JObject.Parse(json);

                if (jsonObj.GetValue("idUser").ToString().Equals(list+""))
                {
                    var playlists = (from p in db.Playlists
                                     orderby p.idPlaylist descending
                                     where p.idUser == list
                                     select new
                                 {
                                      idUser = p.idUser,
                                     namePlayList = p.namePlaylist,
                                     idPlaylist = p.idPlaylist,
                                     imagePlaylist = p.imagePlaylist,
                                     created_date = p.created_date,
                                 }).Take(10);
                    return playlists.ToList();
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

        //lấy ra 1 list
        // GET: api/PlaylistsApi/5
        [ResponseType(typeof(Playlist))]
        public IEnumerable<Object> GetPlaylist(int id, string token, Int32 list)
        {
            //Playlist playlist = db.Playlists.Find(id);
            //if (playlist == null)
            //{
            //    return NotFound();
            //}

            //return Ok(playlist);

            JObject jsonObj = null;
            try
            {
                string json = new JwtBuilder()
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token);

                jsonObj = JObject.Parse(json);

                if (jsonObj.GetValue("idUser").ToString().Equals(list + ""))
                {
                    var playlists = (from p in db.DetailLists
                                     join s in db.Songs on p.idSong equals s.idSong
                                     join a in db.Authors on s.idSinger equals a.idSinger
                                     join k in db.Kinds on s.idKind equals k.idKind
                                     orderby p.idDetailList descending
                                     where p.idList == id
                                     select new
                                     {
                                         idDetail = p.idDetailList,
                                         idSong = s.idSong,
                                         idKind = s.idKind,
                                         idSinger = s.idSinger,
                                         nameSinger = a.nameSinger,
                                         nameSong = s.nameSong,
                                         luotNghe = s.luotNghe,
                                         urlSong = s.urlSong,
                                         imageSong = s.imageSong
                                     }).Take(10);
                    return playlists.ToList();
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

        // PUT: api/PlaylistsApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPlaylist(int id, Playlist playlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != playlist.idPlaylist)
            {
                return BadRequest();
            }

            db.Entry(playlist).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(id))
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

        // POST: api/PlaylistsApi
        [ResponseType(typeof(Playlist))]
        public IHttpActionResult PostPlaylist(Playlist playlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Playlists.Add(playlist);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = playlist.idPlaylist }, playlist);
        }
        // POST: api/PlaylistsApi
        [ResponseType(typeof(Playlist))]
        [HttpPost]
        public IHttpActionResult AddSongToPlaylist(DetailList detail, Int32 idUser, string token)
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
                        return BadRequest(ModelState);
                    }

                    db.DetailLists.Add(detail);
                    db.SaveChanges();

                    return Json(new Models.TokenUser(
                                  200,
                                   "ok",
                                   0
                                  ));
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

        // DELETE: api/PlaylistsApi/5
        [HttpDelete]
        public IHttpActionResult DeletePlaylist(int id, string token, Int32 user)
        {
            JObject jsonObj = null;
            try
            {
                string json = new JwtBuilder()
                   .WithSecret(secret)
                   .MustVerifySignature()
                   .Decode(token);

                jsonObj = JObject.Parse(json);
                if (jsonObj.GetValue("idUser").ToString().Equals(user + "")) {
                    Playlist playlist = db.Playlists.Find(id);
                    DetailList detail = db.DetailLists.Where(b => b.idList == id)
                   .FirstOrDefault();
                    if (playlist == null)
                    {
                        return Json(new Models.TokenUser(
                                  400,
                                   "error",
                                   0
                                  ));
                    }
                    if(detail != null)
                    {
                        db.DetailLists.Remove(detail);
                        db.Playlists.Remove(playlist);
                        db.SaveChanges();
                        return Json(new Models.TokenUser(
                                  200,
                                   "ok",
                                   0
                                  ));
                    }else
                    {
                        db.Playlists.Remove(playlist);
                        db.SaveChanges();
                        return Json(new Models.TokenUser(
                                  200,
                                   "ok",
                                   0
                                  ));
                    }
                }
            }
            catch(Exception e)
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
        [HttpDelete]
        public IHttpActionResult deleteSong(Int32 idList,Int32 idDetail, string token, Int32 idUser)
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
                    DetailList detail = db.DetailLists.Where(b => b.idList == idList && b.idDetailList == idDetail)
                    .FirstOrDefault();
                    if (detail != null)
                    {
                        try
                        {
                            db.DetailLists.Remove(detail);
                            db.SaveChanges();
                            return Json(new Models.TokenUser(
                                         200,
                                          "ok",
                                          idUser
                                         ));
                        }
                        catch (Exception a)
                        {
                            return Json(new Models.TokenUser(
                                         400,
                                          "error",
                                          0
                                         ));
                        }
                    }else
                    {
                        return Json(new Models.TokenUser(
                                         400,
                                          "error",
                                          0
                                         ));
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlaylistExists(int id)
        {
            return db.Playlists.Count(e => e.idPlaylist == id) > 0;
        }
    }
}