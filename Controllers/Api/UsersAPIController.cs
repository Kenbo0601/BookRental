using BookRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BookRental.Controllers.Api
{
    public class UsersAPIController : ApiController
    {
        private ApplicationDbContext db;

        public UsersAPIController()
        {
            db = ApplicationDbContext.Create();
        }
        
        //To Retrieve Email or (Name & BirthDate)
        public IHttpActionResult Get(string type,  string query=null)
        {
            if(type.Equals("email") && query != null)
            {
                //retrieve customers object from db based on email match
                var customerQuery = db.Users.Where(u => u.Email.ToLower().Contains(query.ToLower()));

                return Ok(customerQuery.ToList());
            }
            if(type.Equals("name") && query != null)
            {
                //retrieve customers object from db based on email match
                var customerQuery = from u in db.Users
                                    where u.Email.Contains(query)
                                    select new { u.FirstName, u.LastName, u.BirthDate };

                return Ok(customerQuery.ToList()[0].FirstName + " " + customerQuery.ToList()[0].LastName + ";" + customerQuery.ToList()[0].BirthDate);
            }

            return BadRequest();
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
