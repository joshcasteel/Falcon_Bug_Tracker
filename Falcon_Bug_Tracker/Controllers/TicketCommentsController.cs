using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Falcon_Bug_Tracker.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace Falcon_Bug_Tracker.Controllers
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketComments
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            if(User.IsInRole("Admin"))
            {
                return View(db.TicketComments.ToList());
            }

            if (User.IsInRole("ProjectManager"))
            {
                List<TicketComment> assignedTicketComments = new List<TicketComment>();
                var assignedProjects = db.Projects.Where(p => p.ProjectManagerId == userId).ToList();
                var tickets = db.Tickets.ToList();
                
                foreach (var project in assignedProjects)
                {
                    foreach (var ticket in tickets)
                    {
                        //getting tickets for projects assigned to
                        if (ticket.ProjectId == project.Id)
                        {
                            foreach(var comment in ticket.Comments)
                            {
                                assignedTicketComments.Add(comment);
                            }
                        }
                    }
                }
                return View(assignedTicketComments);
            }

            if(User.IsInRole("Developer"))
            {
                var assignedTickets = db.Tickets.Where(t => t.DeveloperId == userId);
                List<TicketComment> assignedTicketComments = new List<TicketComment>();
                foreach(var item in assignedTickets)
                {
                    foreach(var comment in item.Comments)
                    {
                        assignedTicketComments.Add(comment);
                    }
                }
                return View(assignedTicketComments);
            }

            if(User.IsInRole("Submitter"))
            {
                return View(db.TicketComments.Where(t => t.UserId == userId));
            }
            return View();
        }

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,UserId,Body,Created")] TicketComment ticketComment)
        {
            
            if (ModelState.IsValid)
            {
                ticketComment.Created = DateTime.Now;
                ticketComment.UserId = User.Identity.GetUserId();
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }

            bool allowedEdit = false;
            var userId = User.Identity.GetUserId();

            if (User.IsInRole("Admin"))
            {
                allowedEdit = true;
            }

            if (User.IsInRole("ProjectManager"))
            {
                var assignedProjects = db.Projects.Where(p => p.ProjectManagerId == userId);
                foreach (var project in assignedProjects)
                {
                    foreach (var ticket in project.Tickets)
                    {
                        if (ticketComment.TicketId == ticket.Id)
                        {
                            allowedEdit = true;
                        }
                    }
                }
            }

            if (User.IsInRole("Developer"))
            {
                var ticket = db.Tickets.Find(ticketComment.TicketId);
                if (ticket.DeveloperId == userId)
                {
                    allowedEdit = true;
                }
            }

            if (User.IsInRole("Submitter"))
            {
                if (ticketComment.UserId == userId)
                {
                    allowedEdit = true;
                }
            }

            if (allowedEdit)
            {
                ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketComment.TicketId);
                ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
                return View(ticketComment);
            }
            TempData["Alert"] = "You do not have access to edit that comment";
            return RedirectToAction("Index", "TicketComments", TempData);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,Body,Created")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                    db.Entry(ticketComment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId }); 
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComment);
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
