using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookRental.Models
{
    public class Book
    {
        [Required]
        public int id { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }

        [Required]
        [Range(0,1000)]
        public int Availability { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Required]
        [DisplayFormat(DataFormatString ="{0: MM/dd/yyyy}")]
        public DateTime? DateAdded { get; set; }

        [Required]
        public int GenreId { get; set; }

        public Genre Genre { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0: MM/dd/yyyy}")]
        public DateTime publicationDate { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public string ProductDimensions { get; set; }

        [Required]
        public string Publisher { get; set; }
    }
}