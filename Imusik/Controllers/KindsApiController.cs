using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Imusik;

namespace Imusik.Controllers
{
    public class KindsApiController : ApiController
    {
        private IMUSIKEntities db = new IMUSIKEntities();

        // GET: api/KindsApi
        public IEnumerable<Object> GetKinds()
        {
            var kinds = (from k in db.Kinds
                           select new
                           {
                               nameKind = k.nameKind,
                               idKind = k.idKind
                           }).Take(10);
            return kinds.ToList();
        }

        // GET: api/KindsApi/5
        [ResponseType(typeof(Kind))]
        public IHttpActionResult GetKind(int id)
        {
            Kind kind = db.Kinds.Find(id);
            if (kind == null)
            {
                return NotFound();
            }

            return Ok(kind);
        }

        // PUT: api/KindsApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutKind(int id, Kind kind)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != kind.idKind)
            {
                return BadRequest();
            }

            db.Entry(kind).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KindExists(id))
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

        // POST: api/KindsApi
        [ResponseType(typeof(Kind))]
        public IHttpActionResult PostKind(Kind kind)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Kinds.Add(kind);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = kind.idKind }, kind);
        }

        // DELETE: api/KindsApi/5
        [ResponseType(typeof(Kind))]
        public IHttpActionResult DeleteKind(int id)
        {
            Kind kind = db.Kinds.Find(id);
            if (kind == null)
            {
                return NotFound();
            }

            db.Kinds.Remove(kind);
            db.SaveChanges();

            return Ok(kind);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool KindExists(int id)
        {
            return db.Kinds.Count(e => e.idKind == id) > 0;
        }
    }
}