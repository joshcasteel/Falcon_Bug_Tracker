using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Falcon_Bug_Tracker.Models
{
    public class TicketType
    {
        [Display(Name = "Ticket Type")]
        public int Id { get; set; }
        public string Name { get; set; }
        
    }
}