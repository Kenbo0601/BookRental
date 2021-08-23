using BookRental.Models;
using BookRental.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookRental.ViewModel
{
    public class BookRentalViewModel
    {
        public int Id { get; set; }
        
        //Book Details
        public int BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }

        [Range(0,1000)]
        public int Availability { get; set; }

        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [DisplayFormat(DataFormatString ="{0: MMM dd yyyy")]
        [DisplayName("Date Added")]
        public DateTime? DateAdded { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0: MMM dd yyyy}")]
        [DisplayName("Publication Date")]
        public DateTime publicationDate { get; set; }
        public int Pages { get; set; }

        [DisplayName("Product Dimentions")]
        public string ProductDimensions { get; set; }
        public string Publisher { get; set; }


        //Rental Details 
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0: MMM dd yyyy}")]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0: MMM dd yyyy}")]
        [DisplayName("Actual End Date")]
        public DateTime? ActualEndDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0: MMM dd yyyy}")]
        [DisplayName("Scheduled End Date")]
        public DateTime? ScheduledEndDate { get; set; }
        
        [DisplayName("Additional Charge")]
        public double? AdditionalCharge { get; set; }

        [DisplayName("Rentail Price")]
        public double RentalPrice { get; set; }

        public string RentalDuration { get; set; }
        
        public string Status { get; set; }

        public double rentalPriceOneMonth { get; set; }
        public double rentalPriceSixMonth { get; set; }
        

        //User details
        public string UserId { get; set; }
        public string Email { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public string Name { get { return FirstName + " " + LastName; } }

        [DisplayName("Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0: MMM dd yyyy}")]
        public DateTime BirthDate { get; set; }

        public string actionName
        {
            get
            {
                if (Status.ToLower().Contains(SD.RequestedLower))
                {
                    return "Approve";
                }
                if (Status.ToLower().Contains(SD.ApprovedLower))
                {
                    return "PickUp";
                }
                if (Status.ToLower().Contains(SD.RentedLower))
                {
                    return "Return";
                }
                return null;
            }
        }
    }
}