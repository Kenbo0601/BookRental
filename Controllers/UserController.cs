using BookRental.Models;
using BookRental.Utility;
using BookRental.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BookRental.Controllers
{
    [Authorize(Roles = SD.AdminUserRole)] //Admin controller: only admin is allowed
    public class UserController : Controller
    {
        private ApplicationDbContext db;
    
        //Constructor
        public UserController()
        {
            db = ApplicationDbContext.Create();
        }

        // GET: User
        public ActionResult Index()
        {
            //retrieve data from db and pass it to view
            var user = from u in db.Users
                       join m in db.MembershipTypes on u.MembershipTypeId equals m.Id
                       select new UserViewModel
                       {
                           Id = u.Id,
                           FirstName = u.FirstName,
                           LastName = u.LastName,
                           Email = u.Email,
                           Phone = u.Phone,
                           BirthDate = u.BirthDate,
                           MembershipTypeId = u.MembershipTypeId,
                           MembershipTypes = (ICollection<MembershipType>)db.MembershipTypes.ToList().Where(n => n.Id.Equals(u.MembershipTypeId)),
                           Disable = u.Disable
                       };
            
            //Convert user to list
            var usersList = user.ToList();

            return View(usersList);
        }

        //EDIT Get
        public ActionResult Edit(string id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = db.Users.Find(id);
            if(user == null)
            {
                return HttpNotFound();
            }

            UserViewModel model = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Id = user.Id,
                MembershipTypeId = user.MembershipTypeId,
                MembershipTypes = db.MembershipTypes.ToList(),
                Phone = user.Phone,
                Disable = user.Disable
            };

            return View(model);
        }


        //POST Method for EDIT Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel user)
        {
            if(!ModelState.IsValid)
            {
                UserViewModel model = new UserViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Id = user.Id,
                    MembershipTypeId = user.MembershipTypeId,
                    MembershipTypes = db.MembershipTypes.ToList(),
                    Phone = user.Phone,
                    Disable = user.Disable
                };

                return View("Edit", model);
            }
            else
            {
                var userInDb = db.Users.Single(u => u.Id == user.Id);
                userInDb.FirstName = user.FirstName;
                userInDb.LastName = user.LastName;
                userInDb.Email = user.Email;
                userInDb.BirthDate = user.BirthDate;
                userInDb.MembershipTypeId = user.MembershipTypeId;
                userInDb.Phone = user.Phone;
                userInDb.Disable = user.Disable;
            }
            
            //update the database
            db.SaveChanges();

            return RedirectToAction("Index", "User");
        }

        public ActionResult Details(string id)
        {
            if(id == null || id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = db.Users.Find(id);

            UserViewModel model = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Id = user.Id,
                MembershipTypeId = user.MembershipTypeId,
                MembershipTypes = db.MembershipTypes.ToList(),
                Phone = user.Phone,
                Disable = user.Disable
            };

            return View(model);
        }


        //DELETE Get
        public ActionResult Delete(string id)
        {
            if(id == null || id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = db.Users.Find(id);

            UserViewModel model = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Id = user.Id,
                MembershipTypeId = user.MembershipTypeId,
                MembershipTypes = db.MembershipTypes.ToList(),
                Phone = user.Phone,
                Disable = user.Disable
            };

            return View(model);
        }

        //DELETE Post Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var userInDb = db.Users.Find(id);
            if(id == null || id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userInDb.Disable = true;
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
        }
    }
}