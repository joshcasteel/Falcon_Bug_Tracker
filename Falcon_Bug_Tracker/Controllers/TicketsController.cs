using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Falcon_Bug_Tracker.Helpers;
using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.ViewModel;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace Falcon_Bug_Tracker.Controllers
{
    
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private HistoryHelper historyHelper = new HistoryHelper();
        private NotificationHelper notificationHelper = new NotificationHelper();
        // GET: Tickets
        
        public ActionResult ClearAll()
        {
            var userId = User.Identity.GetUserId();
            var userNotifications = db.TicketNotifications.Where(t => t.ReceipientId == userId);
            foreach(var notificiation in userNotifications)
            {
                notificiation.IsRead = true;
            }
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Index(int? id)
        {
            if(id != null)
            {
                db.TicketNotifications.Find(id).IsRead = true;
                db.SaveChanges();
            }
            var ticketIndexVM = new TicketIndexVM();
            ticketIndexVM.AllTickets = db.Tickets.ToList();            
            
            var userId = User.Identity.GetUserId();
            
            
            if(User.IsInRole("Admin"))
            {
                ticketIndexVM.AssignedTickets = db.Tickets.ToList();
                return View(ticketIndexVM);
            }

            if(User.IsInRole("ProjectManager"))
            {
             
                ticketIndexVM.ProjectTickets = db.Projects.Where(p => p.ProjectManagerId == userId).SelectMany(t => t.Tickets).ToList();
                
                return View(ticketIndexVM);
            }

            if(User.IsInRole("Developer"))
            {
                ticketIndexVM.AssignedTickets = db.Tickets.Where(t => t.DeveloperId == userId).ToList();
                ticketIndexVM.ProjectTickets = db.Users.Find(userId).Projects.SelectMany(t => t.Tickets).ToList();
                
                return View(ticketIndexVM);
            }

            if(User.IsInRole("Submitter"))
            {
                ticketIndexVM.AssignedTickets = db.Tickets.Where(t => t.SubmitterId == userId).ToList();
                return View(ticketIndexVM);
            }

            
            return View(ticketIndexVM);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Tickets.Find(id);

            if (ticket == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            TicketDetailsVM ticketDetails = new TicketDetailsVM();

            if (ticket.DeveloperId != null)
                ticketDetails.Developer = db.Users.Find(ticket.DeveloperId).FullName;
            
            if(ticket.Project.ProjectManagerId != null)
                ticketDetails.ProjectManager = db.Users.Find(ticket.Project.ProjectManagerId).FullName;

            ticketDetails.TicketPriority = db.TicketPriorities.Find(ticket.TicketPriorityId).Name;
            ticketDetails.TicketStatus = db.TicketStatuses.Find(ticket.TicketStatusId).Name;
            ticketDetails.TicketType = db.TicketTypes.Find(ticket.TicketTypeId).Name;
            ticketDetails.ProjectName = db.Projects.Find(ticket.ProjectId).Name;
            ticketDetails.Submitter = db.Users.Find(ticket.SubmitterId).FullName;
            ticketDetails.Ticket = ticket;
            ticketDetails.TicketHistory = db.TicketHistories.Where(t => t.TicketId == ticket.Id).OrderByDescending(d => d.ChangedOn).ToList();
            ticketDetails.ticketAttachments = db.TicketAttachments.Where(t => t.TicketId == ticket.Id).ToList();

            foreach (var comment in ticket.Comments)
            {
                comment.UserId = db.Users.Find(comment.UserId).FullName;
            }

            if (User.IsInRole("Admin"))
            {
                return View(ticketDetails);
            }

            if (User.IsInRole("ProjectManager"))
            {
                var assignedProjects = db.Projects.Where(p => p.ProjectManagerId == userId).ToList();
                foreach(var project in assignedProjects)
                {
                    if(project.Id == ticket.ProjectId)
                    {
                        return View(ticketDetails);
                    }
                    TempData["Alert"] = "You can only view tickets for your assigned projects";
                    return RedirectToAction("Index", "Tickets", TempData);
                }
            }

            if(User.IsInRole("Developer"))
            {
                if(ticket.DeveloperId == userId)
                {
                    return View(ticketDetails);
                }
                TempData["Alert"] = "You can only view tickets you've been assigned";
                return RedirectToAction("Index", "Tickets", TempData);
            }

            if (User.IsInRole("Submitter"))
            {
                if (ticket.SubmitterId == userId)
                {
                    return View(ticketDetails);
                }
                TempData["Alert"] = "You can only view tickets you've created";
                return RedirectToAction("Index", "Tickets", TempData);
            }

            TempData["Alert"] = "You don't have access to ticket details";
            return RedirectToAction("Index", "Tickets", TempData);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            if(User.IsInRole("Submitter"))
            {
                var userId = User.Identity.GetUserId();
                var submitterProjects = projHelper.ListUserProjects(userId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
                ViewBag.ProjectId = new SelectList(submitterProjects, "Id", "Name");
                ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FullName");
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
                return View();
            }
            TempData["Alert"] = "You are not authorized to create tickets";
            return RedirectToAction("Index", "Tickets");
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectId,TicketTypeId,TicketPriorityId,Title,Description")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.SubmitterId = User.Identity.GetUserId();
                ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(t => t.Name == "Open").Id;
                ticket.Created = DateTime.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FullName", ticket.DeveloperId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FullName", ticket.SubmitterId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            TicketInfoVM editTicketData = new TicketInfoVM();
            editTicketData.Title = ticket.Title;
            editTicketData.Description = ticket.Description;
            editTicketData.TicketId = ticket.Id;
            editTicketData.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            editTicketData.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            editTicketData.DeveloperId = new SelectList(rolesHelper.UsersInRole("Developer"), "Id", "FullName", ticket.DeveloperId);
            editTicketData.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                
                return View(editTicketData);
            }

            if (User.IsInRole("ProjectManager"))
            {
                var assignedProjects = db.Projects.Where(p => p.ProjectManagerId == userId).ToList();
                foreach (var project in assignedProjects)
                {
                    if (project.Id == ticket.ProjectId)
                    {
                        return View(editTicketData);
                    }
                    TempData["Alert"] = "You can only edit tickets for your assigned projects";
                    return RedirectToAction("Index", "Tickets", TempData);
                }
            }

            if(User.IsInRole("Developer"))
            {
                if(ticket.DeveloperId == userId)
                {
                    return View(editTicketData);
                }
                TempData["Alert"] = "You can only edit tickets assigned to you";
                return RedirectToAction("Index", "Tickets", TempData);
            }

            if(User.IsInRole("Submitter"))
            {
                if(ticket.SubmitterId == userId)
                {
                    return View(editTicketData);
                }
                TempData["Alert"] = "You can only edit tickets you've created";
                return RedirectToAction("Index", "Tickets", TempData);
            }

            TempData["Alert"] = "You do not have access to edit tickets";
            return RedirectToAction("Index", "Tickets", TempData);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int Id, int TicketTypeId, int TicketStatusId, int TicketPriorityId, string Title, string Description, string DeveloperId)
        {
            if (ModelState.IsValid)
            {
                //AsNoTracking() to get a Memento Ticket Object
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == Id);


                var newTicket = db.Tickets.Find(Id);
                newTicket.Title = Title;
                newTicket.Description = Description;
                newTicket.TicketPriorityId = TicketPriorityId;
                newTicket.TicketStatusId = TicketStatusId;
                newTicket.DeveloperId = DeveloperId == "" ? null : DeveloperId;
                newTicket.TicketTypeId = TicketTypeId;
                newTicket.Updated = DateTime.Now;

                db.SaveChanges();


                historyHelper.ManageHistoryRecordCreation(oldTicket, newTicket);
                notificationHelper.ManageNotifications(oldTicket, newTicket);

                return RedirectToAction("Index");
            }
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FullName", Id);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", TicketTypeId);
            return View();
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
