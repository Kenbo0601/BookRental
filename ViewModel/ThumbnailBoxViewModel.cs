using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookRental.Models;

namespace BookRental.ViewModel
{
    public class ThumbnailBoxViewModel
    {
        public IEnumerable<ThumbnailModel> Thumbnails { get; set; }
    }
}