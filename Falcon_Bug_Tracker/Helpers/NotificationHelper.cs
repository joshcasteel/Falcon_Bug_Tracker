using Falcon_Bug_Tracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.Helpers
{
    public class NotificationHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void ManageNotifications(Ticket oldTicket, Ticket newTicket)
        {
            //Developer Assignment Notification
            GenerateTicketAssignmentNotifications(oldTicket, newTicket);
            //General Change Notification
            GenerateTicketChangeNotification(oldTicket, newTicket);
        }
        private void GenerateTicketAssignmentNotifications(Ticket oldTicket, Ticket newTicket)
        {
            bool assigned = oldTicket.DeveloperId == null && newTicket.DeveloperId != null;
            bool unassigned = oldTicket.DeveloperId != null && newTicket.DeveloperId == null;
            bool reassigned = !assigned && !unassigned && oldTicket.DeveloperId != newTicket.DeveloperId;

            if(assigned)
            {
                var created = DateTime.Now;
                db.TicketNotifications.Add(new TicketNotification
                {
                    TicketId = newTicket.Id,
                    Created = created,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    RecipientId = newTicket.DeveloperId,
                    Subject = $"You've been assigned ticket {newTicket.Id}",
                    NotificationBody = $"{newTicket.Title} has been assigned to you at ${newTicket.Updated}."
                }) ;
                db.SaveChanges();
            }

            if (unassigned)
            {
                var created = DateTime.Now;
                db.TicketNotifications.Add(new TicketNotification
                {
                    TicketId = newTicket.Id,
                    Created = created,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    RecipientId = oldTicket.DeveloperId,
                    Subject = $"You've unassigned from ticket {newTicket.Id}",
                    NotificationBody = $"{oldTicket.Title} has been unassigned to you at ${newTicket.Updated}."
                });
                db.SaveChanges();
            }

            if (reassigned)
            {
                var created = DateTime.Now;
                db.TicketNotifications.Add(new TicketNotification
                {
                    TicketId = newTicket.Id,
                    Created = created,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    RecipientId = newTicket.DeveloperId,
                    Subject = $"You've been assigned ticket {newTicket.Id}",
                    NotificationBody = $"{newTicket.Title} has been assigned to you at ${newTicket.Updated}."
                });

                db.TicketNotifications.Add(new TicketNotification
                {
                    TicketId = newTicket.Id,
                    Created = created,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    RecipientId = oldTicket.DeveloperId,
                    Subject = $"You've unassigned from ticket {newTicket.Id}",
                    NotificationBody = $"{oldTicket.Title} has been assigned to you at ${newTicket.Updated}."
                });
                db.SaveChanges();
            }
        }

        private void GenerateTicketChangeNotification(Ticket oldTicket, Ticket newTicket)
        {
            
        }

        public class TopBarNotifications
        {
            public static List<TicketNotification> GetUnreadNotifications()
            {
                var userId = HttpContext.Current.User.Identity.GetUserId();
                
                if(userId == null)
                {
                    return new List<TicketNotification>();
                }

                var db = new ApplicationDbContext();
                var notifications = db.TicketNotifications.Where(t => t.RecipientId == userId && !t.IsRead).ToList();
                return notifications;

            }
        }

    }
}