using BookRental.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BookRental.Controllers
{
    public class GenreController : Controller
    {
        private ApplicationDbContext db;

        public GenreController()
        {
            db = new ApplicationDbContext();
        }

        // GET: Genre
        public ActionResult Index()
        {
            return View(db.Genres.ToList());
        }
        
        //Get Action
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Genre genre)
        {
            //check required attributes in genre.cs
            if(ModelState.IsValid)
            {
                //not good ideal to access to the database here. Usually implement DAL layer (Repository pattern)
                db.Genres.Add(genre);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Accessing the database
            Genre genre = db.Genres.Find(id);
            if(genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Accessing the database
            Genre genre = db.Genres.Find(id);
            if(genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Genre genre)
        {
            if(ModelState.IsValid)
            {
                //alternative
                //var GenreInDb = db.Genres.FirstOrDefault(g => g.Id.Equals(genre.Id));
                //GenreInDb.Name = genre.Name;

                db.Entry(genre).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Accessing the database
            Genre genre = db.Genres.Find(id);
            if(genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Genre genre = db.Genres.Find(id);
            db.Genres.Remove(genre);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }
    }
}