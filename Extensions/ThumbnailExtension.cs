using BookRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookRental.Extensions
{
    public static class ThumbnailExtension
    {
        public static IEnumerable<ThumbnailModel> GetBookThumnail(this List<ThumbnailModel> thumbnails, ApplicationDbContext db=null, string search = null)
        {
            try
            {
                if (db == null)
                {
                    db = ApplicationDbContext.Create();
                }

                thumbnails = (from b in db.Books
                              select new ThumbnailModel
                              {
                                  Id = b.id,
                                  Title = b.Title,
                                  Description = b.Description,
                                  ImageUrl = b.ImageUrl,
                                  Link = "/BookDetail/Index/" + b.id
                              }).ToList();

                if(search != null)
                {
                    return thumbnails.Where(t => t.Title.ToLower().Contains(search.ToLower())).OrderBy(t => t.Title);
                }
            }
            catch(Exception ex)
            {

            }

            return thumbnails.OrderBy(b => b.Title);
        }
    }
}