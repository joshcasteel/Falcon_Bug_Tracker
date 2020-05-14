using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class CustomTicketInfo
    {
        public SelectList Developer { get; set; }
        
        public SelectList TicketPriorityId { get; set; }
        public SelectList TicketStatusId { get; set; }
        public SelectList TicketTypeId { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public int TicketId { get; set; }

    }
}