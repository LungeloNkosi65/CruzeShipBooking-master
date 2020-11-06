using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CruzeShipBooking.Models
{
    public class Cruise
    {
        [Key]
        public int CruiseId { get; set; }
        [DisplayName("Cruise Name")]
        public string CruiseName { get; set; }
        [DisplayName("No. Passangers")]
        public int NumberOfPassagers { get; set; }
        public string Status { get; set; }
        //
    }
}