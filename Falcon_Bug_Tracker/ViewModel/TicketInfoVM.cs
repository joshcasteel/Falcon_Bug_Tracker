using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class TicketInfoVM
    {
        public SelectList DeveloperId { get; set; }
        
        public SelectList TicketPriorityId { get; set; }
        public SelectList TicketStatusId { get; set; }
        public SelectList TicketTypeId { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public int TicketId { get; set; }

    }

    public class TicketCreateVM
    {
        public Ticket Ticket { get; set; }
        public List<ApplicationUser> Developers { get; set; }
        public string DeveloperId { get; set; }

    }

    public class TicketDetailsVM
    {
        public Ticket Ticket { get; set; }
        public string ProjectName { get; set; }
        public string ProjectManager { get; set; }
        public string Developer { get; set; }
        public string Submitter { get; set; }
        public string TicketPriority { get; set; }
        public string TicketStatus { get; set; }
        public string TicketType { get; set; }
        public IEnumerable<TicketHistory> TicketHistory { get; set; }        
        public IEnumerable<TicketAttachment> ticketAttachments { get; set; }
    }

    public class TicketIndexVM
    {
        public IEnumerable<Ticket> AllTickets { get; set; }
        public IEnumerable<Ticket> AssignedTickets { get; set; }
        public IEnumerable<Ticket> ProjectTickets { get; set; }
        
    }
}