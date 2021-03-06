using BookRental.Models;
using BookRental.Utility;
using BookRental.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;

namespace BookRental.Controllers
{
    [Authorize] //does not allow anyone to access to the bookrent page unless they are logged in
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
                    Status = BookRent.StatusEnum.Approved,
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
        public ActionResult Index(int? pageNumber, string option=null, string search=null)
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
            
            //if user checks the radioButton (Index.cshtml) search for email from the database
            if(option == "email" && search.Length > 0)
            {
                model = model.Where(u => u.Email.Contains(search));
            }
            if(option == "name" && search.Length > 0)
            {
                model = model.Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search));
            }
            if(option == "status" && search.Length > 0)
            {
                model = model.Where(u => u.Status.Contains(search));
            }
            
            //check if the user is admin. if not, user can only see their books
            if(!User.IsInRole(SD.AdminUserRole))
            {
                model = model.Where(u => u.UserId.Equals(userid));
            }

            return View(model.ToList().ToPagedList(pageNumber ?? 1,10)); //10 rows for each page 
        }
        
        [HttpPost]
        public ActionResult Reserve(BookRentalViewModel book)
        {
            var userid = User.Identity.GetUserId();
            Book bookToRent = db.Books.Find(book.BookId);
            double rentalPr = 0;

            if(userid != null)
            {
                 var chargeRate = from u in db.Users
                                 join m in db.MembershipTypes on u.MembershipTypeId equals m.Id
                                 where u.Id.Equals(userid)
                                 select new { m.ChargeRateOneMonth, m.ChargeRateSixMonth };

                if(book.RentalDuration == SD.SixMonthCount)
                {
                    rentalPr = Convert.ToDouble(bookToRent.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateSixMonth) / 100;
                }
                else
                {
                    rentalPr = Convert.ToDouble(bookToRent.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateOneMonth) / 100;
                }

                BookRent bookRent = new BookRent
                {
                    BookId = bookToRent.id,
                    UserId = userid,
                    RentalDuration = book.RentalDuration,
                    RentalPrice = rentalPr,
                    Status = BookRent.StatusEnum.Requested
                };

                db.BookRental.Add(bookRent);

                var bookInDb = db.Books.SingleOrDefault(c => c.id == book.BookId);
                bookInDb.Availability -= 1;
                db.SaveChanges();
                return RedirectToAction("Index", "BookRent");
            }


            return View();
        }

        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //find a rental record from dabase based on the id
            BookRent bookRent = db.BookRental.Find(id);

            var model = getVMFromBookRent(bookRent);

            if(model==null)
            {
                return HttpNotFound();
            }

            return View(model);
        }
        
        //Decline GET
        public ActionResult Decline(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //find a rental record from dabase based on the id
            BookRent bookRent = db.BookRental.Find(id);

            var model = getVMFromBookRent(bookRent);

            if(model==null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Decline(BookRentalViewModel model)
        {
            //invalid request
            if(model.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Find the book from the database and update the satatus to rejected
            BookRent bookRent = db.BookRental.Find(model.Id);
            bookRent.Status = BookRent.StatusEnum.Rejected;
            
            //Find the book from the database and modify the num of availability
            Book bookInDb = db.Books.Find(bookRent.BookId);
            bookInDb.Availability += 1;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
        
        //Approve GET
        public ActionResult Approve(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //find a rental record from dabase based on the id
            BookRent bookRent = db.BookRental.Find(id);

            var model = getVMFromBookRent(bookRent);

            if(model==null)
            {
                return HttpNotFound();
            }

            return View("Approve",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(BookRentalViewModel model)
        {
            //invalid request
            if(model.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Find the book from the database and update the status
            BookRent bookRent = db.BookRental.Find(model.Id);
            bookRent.Status = BookRent.StatusEnum.Approved;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
        
        //Delete GET
        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //find a rental record from dabase based on the id
            BookRent bookRent = db.BookRental.Find(id);

            var model = getVMFromBookRent(bookRent);

            if(model==null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int Id)
        {
            //invalid request
            if(Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Find the book from the database 
            BookRent bookRent = db.BookRental.Find(Id);

            var bookInDb = db.Books.Where(b => b.id.Equals(bookRent.BookId)).FirstOrDefault();
            if(!bookRent.Status.ToString().Equals("Rented"))
            {
                bookInDb.Availability += 1;
            }

            db.BookRental.Remove(bookRent);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        
        //PickUp GET
        public ActionResult PickUp(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //find a rental record from dabase based on the id
            BookRent bookRent = db.BookRental.Find(id);

            var model = getVMFromBookRent(bookRent);

            if(model==null)
            {
                return HttpNotFound();
            }

            return View("Approve",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PickUp(BookRentalViewModel model)
        {
            //invalid request
            if(model.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Find the book from the database and update the status
            BookRent bookRent = db.BookRental.Find(model.Id);
            bookRent.Status = BookRent.StatusEnum.Rented;
            bookRent.StartDate = DateTime.Now;
            if(bookRent.RentalDuration == SD.SixMonthCount)
            {
                bookRent.ScheduledEndDate = DateTime.Now.AddMonths(Convert.ToInt32(SD.SixMonthCount));
            }
            else
            {
                bookRent.ScheduledEndDate = DateTime.Now.AddMonths(Convert.ToInt32(SD.OneMonthCount));
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Return(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //find a rental record from database based on the id
            BookRent bookRent = db.BookRental.Find(id);

            var model = getVMFromBookRent(bookRent);

            if(model==null)
            {
                return HttpNotFound();
            }

            return View("Approve",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Return(BookRentalViewModel model)
        {
            //invalid request
            if(model.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Find the book from the database and update the status
            BookRent bookRent = db.BookRental.Find(model.Id);
            bookRent.Status = BookRent.StatusEnum.Closed;
            bookRent.AdditionalCharge = model.AdditionalCharge;
            
            //Find the user and increment the rental count for this user
            var user = db.Users.FirstOrDefault(u => u.Id == bookRent.UserId);
            //If user rented books 5 times, reset the counter for the next discount service
            if (user.RentalCount < 5)
            {
                user.RentalCount += 1;
            }
            else
            {
                user.RentalCount = 0; //rentalCount = 5, so reset the counter 
            }

            Book bookInDb = db.Books.Find(bookRent.BookId);
            bookInDb.Availability += 1;

            bookRent.ActualEndDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        
        //Helper function
        private BookRentalViewModel getVMFromBookRent(BookRent bookRent)
        {
            Book bookSelected = db.Books.Where(b => b.id == bookRent.BookId).FirstOrDefault();

            var userDetails = from u in db.Users
                              where u.Id.Equals(bookRent.UserId)
                              select new { u.Id, u.FirstName, u.LastName, u.BirthDate, u.Email };

            BookRentalViewModel model = new BookRentalViewModel
            {
                Id = bookRent.Id,
                BookId = bookSelected.id,
                RentalPrice = bookRent.RentalPrice,
                Price = bookSelected.Price,
                Pages = bookSelected.Pages,
                FirstName = userDetails.ToList()[0].FirstName,
                LastName = userDetails.ToList()[0].LastName,
                BirthDate = userDetails.ToList()[0].BirthDate,
                Email = userDetails.ToList()[0].Email,
                UserId = userDetails.ToList()[0].Id,
                ScheduledEndDate = bookRent.ScheduledEndDate,
                Author = bookSelected.Author,
                StartDate = bookRent.StartDate,
                Availability = bookSelected.Availability,
                DateAdded = bookSelected.DateAdded,
                Description = bookSelected.Description,
                GenreId = bookSelected.GenreId,
                Genre = db.Genres.FirstOrDefault(g => g.Id.Equals(bookSelected.GenreId)),
                ISBN = bookSelected.ISBN,
                ImageUrl = bookSelected.ImageUrl,
                ProductDimensions = bookSelected.ProductDimensions,
                publicationDate = bookSelected.publicationDate,
                Publisher = bookSelected.Publisher,
                RentalDuration = bookRent.RentalDuration,
                Status = bookRent.Status.ToString(),
                Title = bookSelected.Title,
                AdditionalCharge = bookRent.AdditionalCharge
            };

            return model;
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