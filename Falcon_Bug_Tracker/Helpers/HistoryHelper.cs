using Falcon_Bug_Tracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.Helpers
{
    public class HistoryHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void ManageHistoryRecordCreation(Ticket oldTicket, Ticket newTicket)
        {
            if (oldTicket.Title != newTicket.Title)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = HttpContext.Current.User.Identity.GetUserId(),
                    Property = "Title",
                    OldValue = oldTicket.Title,
                    NewValue = newTicket.Title,
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }

            if (oldTicket.Description != newTicket.Description)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = HttpContext.Current.User.Identity.GetUserId(),
                    Property = "Description",
                    OldValue = oldTicket.Description,
                    NewValue = newTicket.Description,
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }

            if (oldTicket.DeveloperId != newTicket.DeveloperId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = HttpContext.Current.User.Identity.GetUserId(),
                    Property = "DeveloperId",
                    OldValue = oldTicket.Developer == null ? "Unassigned" : oldTicket.Developer.FullName,
                    NewValue = newTicket.Developer == null ? "Unassigned" : newTicket.Developer.FullName,
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }

            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = HttpContext.Current.User.Identity.GetUserId(),
                    Property = "TicketPriorityId",
                    OldValue = oldTicket.Priority.Name,
                    NewValue = newTicket.Priority.Name,
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }

            if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = HttpContext.Current.User.Identity.GetUserId(),
                    Property = "TicketStatusId",
                    OldValue = oldTicket.Status.Name,
                    NewValue = newTicket.Status.Name,
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }

            if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = HttpContext.Current.User.Identity.GetUserId(),
                    Property = "TicketTypeId",
                    OldValue = oldTicket.TicketType.Name,
                    NewValue = newTicket.TicketType.Name,
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }

            if (oldTicket.IsArchived != newTicket.IsArchived)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = HttpContext.Current.User.Identity.GetUserId(),
                    Property = "IsArchived",
                    OldValue = oldTicket.IsArchived.ToString(),
                    NewValue = newTicket.IsArchived.ToString(),
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }

            db.SaveChanges();
        }

    }
}