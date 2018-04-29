using Imusik.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Imusik.Controllers
{
    public class HomeController : Controller
    {
        private IMUSIKEntities imusik = new IMUSIKEntities();
        public ActionResult Index()
        {
            if (Request.Cookies["idUser"] != null)
            {
                SongObj songObj = new SongObj()
                {
                    authors = imusik.Authors.ToList(),
                    kinds = imusik.Kinds.ToList(),
                };
                return View(songObj);
            }
            else
            {
                return Redirect("http://192.168.1.38:8081/login");
            }
        }

        public ActionResult Playlist()
        {

            return View();
        }

        public ActionResult Singer()
        {
            return View();
        }
        [HttpPost]
        public void AddSinger(HttpPostedFileBase file)
        {
            String name = Request["name"];
            Author author = new Author()
            {
                nameSinger = name,
                imageSinger = UploadImage.uploadImage(file).ToString()
            };

            imusik.Authors.Add(author);
            imusik.SaveChanges();
            //return Json(new { id = author.idSinger });
            Response.Write("<img src='"+author.imageSinger+"'/>");
        }

        public ActionResult Kind()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddKind()
        {
            String name = Request["name"];
            Kind kind = new Kind() {
                    nameKind = name
            };

            imusik.Kinds.Add(kind);
            imusik.SaveChanges();
            return Json(new { id = kind.idKind});
        }
        [HttpPost]
        public void Upload(HttpPostedFileBase file)
        {
            String id = GoogleDriveFilesRepository.FileUploadInFolder("1JB5HUSkqL41QRquHHD7dzvr9tHkJxtaV", file);
            DateTime userLocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, System.TimeZoneInfo.Local);
            DateTime dateTime = DateTime.UtcNow.Date;
            string date = userLocalNow.ToString("dd-MM-yyyy");
            Song song = new Song()
            {
                nameSong = Request["nameSong"],
                idSinger = int.Parse(Request["singerSong"]),
                idKind = int.Parse(Request["kindSong"]),
                urlSong = "https://drive.google.com/uc?id="+id+"&export=download",
                created_date = date
            };
            imusik.Songs.Add(song);
            imusik.SaveChanges();
            int idSong = song.idSong;
            if (idSong != 0)
            {
                Response.Redirect("http://192.168.1.38:8081/home/Cover/?id=" + idSong);
            }
        }
        [HttpGet]
        public ActionResult Cover()
        {
            ViewBag.Id = Request["id"];
            return View();
        }

        [HttpPost]
        public void AddCover(HttpPostedFileBase file)
        {
            int id = Int32.Parse(Request["id"]);
            var result = imusik.Songs.SingleOrDefault(b => b.idSong == id);
            if (result != null)
            {
                result.imageSong = UploadImage.uploadImage(file).ToString();
                imusik.SaveChanges();
            }
            Response.Redirect("http://192.168.1.38:8081/");
        }
    }
}