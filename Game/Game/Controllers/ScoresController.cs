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
using Game;
using JWT.Builder;

namespace Game.Controllers
{
    public class ScoresController : ApiController
    {
        const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        private NotesEntities db = new NotesEntities();

        // GET: api/Scores
        public IQueryable<Score> GetScores()
        {
            return db.Scores;
        }

        // GET: api/Scores/5
        [ResponseType(typeof(Score))]
        public IEnumerable<Object> GetScore(int id, string token)
        {
            if (checkToken(token))
            {
                var score = (from k in db.Scores
                             where k.idUser == id
                             select new
                             {
                                 score = k.score1
                             }).Take(10);
                return score.ToList();
            }
            return null;
        }

        // PUT: api/Scores/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutScore(int id, Score score)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != score.idScore)
            {
                return BadRequest();
            }

            db.Entry(score).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreExists(id))
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

        // POST: api/Scores
        [ResponseType(typeof(Score))]
        public async Task<IHttpActionResult> PostScore(Score score, string token)
        {
            if (checkToken(token))
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.Scores.Add(score);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (ScoreExists(score.idScore))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtRoute("DefaultApi", new { id = score.idScore }, score);
            }

            return null;
        }

        // DELETE: api/Scores/5
        [ResponseType(typeof(Score))]
        public async Task<IHttpActionResult> DeleteScore(int id)
        {
            Score score = await db.Scores.FindAsync(id);
            if (score == null)
            {
                return NotFound();
            }

            db.Scores.Remove(score);
            await db.SaveChangesAsync();

            return Ok(score);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ScoreExists(int id)
        {
            return db.Scores.Count(e => e.idScore == id) > 0;
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