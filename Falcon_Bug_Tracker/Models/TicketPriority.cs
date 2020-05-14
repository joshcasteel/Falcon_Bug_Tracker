using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.Models
{
    public class TicketPriority
    {
        [Display(Name = "Ticket Priority")]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}