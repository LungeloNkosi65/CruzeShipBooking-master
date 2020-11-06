using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CruzeShipBooking.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [DisplayName("Customer Email")]
        public string CustomerEmail { get; set; }
        [ForeignKey("BookingPackage")]
        public int BookingPackageId { get; set; }
        public virtual BookingPackage BookingPackage { get; set; }
        [DisplayName("Date Booking For"),Required,DataType(DataType.Date)]
        public DateTime DateBookingFor { get; set; }
        [DisplayName("Check In Time"),DataType(DataType.Time)]
        public DateTime CheckInTime { get; set; }
        [DisplayName("Date Booked")]
        public DateTime DateBooked { get; set; }

        [DisplayName("Booking Amount"),DataType(DataType.Currency)]
        public decimal BookingPrice { get; set; }
        [DisplayName("Status")]
        public string BookingStatus { get; set; }
        [DisplayName("Reference Number")]

        public string ReferenceNumber { get; set; }

        public ApplicationDbContext db = new ApplicationDbContext();

        public decimal GetBookingFee()
        {
            var fee = (from bp in db.BookingPackages
                       where bp.BookingPackageId == BookingPackageId
                       select bp.PackagePrice
                     ).FirstOrDefault();
            return fee;
        }
        public bool CheckBookingDate()
        {
            if (DateBookingFor < DateTime.Now.Date)
            {
                return true;
            }
            return false;
        }

       public string GenerateReferenceNumber(string userId)
        {
            string referenceNo = CustomerEmail.Substring(0, CustomerEmail.LastIndexOf("@"))+userId.Substring(0,4)+BookingId;
            return referenceNo;
        }
    }
}