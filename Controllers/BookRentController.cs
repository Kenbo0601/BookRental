using BookRental.Models;
using BookRental.Utility;
using BookRental.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookRental.Controllers
{
    public class BookRentController : Controller
    {
        private ApplicationDbContext db;

        public BookRentController()
        {
            db = ApplicationDbContext.Create();
        }

        public ActionResult Create(string title = null, string ISBN = null)
        {
            if(title != null && ISBN != null)
            {
                BookRentalViewModel model = new BookRentalViewModel
                {
                    Title = title,
                    ISBN = ISBN,
                };

                return View(model);
            }
            return View(new BookRentalViewModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Post Action method
        public ActionResult Create(BookRentalViewModel bookRent)
        {
            if(ModelState.IsValid)
            {
                var email = bookRent.Email;

                var userDetails = from u in db.Users
                                  where u.Email.Equals(email)
                                  select new { u.Id };

                var ISBN = bookRent.ISBN;

                Book bookSelected = db.Books.Where(b => b.ISBN == ISBN).FirstOrDefault();

                var rentalDuration = bookRent.RentalDuration;

                var chargeRate = from u in db.Users
                                 join m in db.MembershipTypes on u.MembershipTypeId equals m.Id
                                 where u.Email.Equals(email)
                                 select new { m.ChargeRateOneMonth, m.ChargeRateSixMonth };

                var oneMonthRental = Convert.ToDouble(bookSelected.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateOneMonth) / 100;
                var sixMonthRental = Convert.ToDouble(bookSelected.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateSixMonth) / 100;

                double rentalPr = 0;

                if(bookRent.RentalDuration == SD.SixMonthCount)
                {
                    rentalPr = sixMonthRental;
                }
                else
                {
                    rentalPr = oneMonthRental;
                }

                BookRent modelToAddToDb = new BookRent
                {
                    BookId = bookSelected.id,
                    RentalPrice = rentalPr,
                    ScheduledEndDate = bookRent.ScheduledEndDate,
                    RentalDuration = bookRent.RentalDuration,
                    Status = BookRent.StatusEnum.Requested,
                    UserId = userDetails.ToList()[0].Id
                };

                bookSelected.Availability -= 1;
                db.BookRental.Add(modelToAddToDb);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: BookRent
        public ActionResult Index()
        {
            string userid = User.Identity.GetUserId();

            var model = from br in db.BookRental
                        join b in db.Books on br.BookId equals b.id
                        join u in db.Users on br.UserId equals u.Id

                        select new BookRentalViewModel
                        {
                            BookId = b.id,
                            RentalPrice = br.RentalPrice,
                            Price = b.Price,
                            Pages = b.Pages,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            BirthDate = u.BirthDate,
                            ScheduledEndDate = br.ScheduledEndDate,
                            Author = b.Author,
                            Availability = b.Availability,
                            DateAdded = b.DateAdded,
                            Description = b.Description,
                            Email = u.Email,
                            GenreId = b.GenreId,
                            Genre = db.Genres.Where(g => g.Id.Equals(b.GenreId)).FirstOrDefault(),
                            ISBN = b.ISBN,
                            ImageUrl = b.ImageUrl,
                            ProductDimensions = b.ProductDimensions,
                            publicationDate = b.publicationDate,
                            Publisher = b.Publisher,
                            RentalDuration = br.RentalDuration,
                            Status = br.Status.ToString(),
                            Title = b.Title,
                            UserId = u.Id,
                            Id = br.Id,
                            StartDate = br.StartDate
                        };
            
            //check if the user is admin. if not, user can only see their books
            if(!User.IsInRole(SD.AdminUserRole))
            {
                model = model.Where(u => u.UserId.Equals(userid));
            }

            return View(model.ToList());
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