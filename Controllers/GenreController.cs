using BookRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }
    }
}