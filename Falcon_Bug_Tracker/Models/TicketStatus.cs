using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Falcon_Bug_Tracker.Models
{
    public class TicketStatus
    {
        [Display(Name = "Ticket Status")]
        public int Id { get; set; }
        public string Name { get; set; }

    }
}