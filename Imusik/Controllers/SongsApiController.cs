using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Imusik;

namespace Imusik.Controllers
{
    public class SongsApiController : ApiController
    {
        private IMUSIKEntities db = new IMUSIKEntities();

        // GET: api/SongsApi
        public IEnumerable<Object> GetSongs(int page)
        {
            var songs = (from s in db.Songs
                         join a in db.Authors on s.idSinger equals a.idSinger
                         orderby s.idSong descending
                         select new
                         {
                             nameSinger = a.nameSinger,
                             nameSong = s.nameSong,
                             idSong = s.idSong,
                             idKind = s.idKind,
                             idSinger = s.idSinger,
                             urlSong = s.urlSong,
                             imageSong = s.imageSong
                         }).Skip(10 * page).Take(10);
            return songs.ToList();
        }

        // GET: api/SongsApi/5
        [ResponseType(typeof(Song))]
        public IEnumerable<Object> GetSong(int id)
        {
            var songs = (from s in db.Songs
                         join a in db.Authors on s.idSinger equals a.idSinger
                         join k in db.Kinds on s.idKind equals k.idKind
                         orderby s.idSong
                         where s.idSong == id
                         select new
                         {
                             nameSinger = a.nameSinger,
                             nameSong = s.nameSong,
                             nameKind = k.nameKind,
                             idSong = s.idSong,
                             idKind = s.idKind,
                             idSinger = s.idSinger,
                             urlSong = s.urlSong,
                             imageSong = s.imageSong
                         });
            return songs.ToList();
        }

        // PUT: api/SongsApi/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSong(int id, Song song)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != song.idSong)
            {
                return BadRequest();
            }

            db.Entry(song).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
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

        // POST: api/SongsApi
        [ResponseType(typeof(Song))]
        public async Task<IHttpActionResult> PostSong(Song song)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Songs.Add(song);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = song.idSong }, song);
        }

        // DELETE: api/SongsApi/5
        [ResponseType(typeof(Song))]
        public async Task<IHttpActionResult> DeleteSong(int id)
        {
            Song song = await db.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            db.Songs.Remove(song);
            await db.SaveChangesAsync();

            return Ok(song);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SongExists(int id)
        {
            return db.Songs.Count(e => e.idSong == id) > 0;
        }
    }
}