using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.Helpers;
using Falcon_Bug_Tracker.ViewModel;
using System.Web.Security;

namespace Falcon_Bug_Tracker.Controllers
{
    
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        [AllowAnonymous]
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("CustomLogOff", "Account");
            }
            return View();
        }
        [Authorize]
        public ActionResult Dashboard()
        {
            var projectsDashboardVM = new ProjectDashboardVM();
            projectsDashboardVM.NumberOfProjects = db.Projects.Count();
            projectsDashboardVM.NumberOfTickets = db.Tickets.Count();
            projectsDashboardVM.NumberOfProjectManagers = rolesHelper.UsersInRole("ProjectManager").Count();
            projectsDashboardVM.NumberOfDevelopers = rolesHelper.UsersInRole("Developer").Count();
            projectsDashboardVM.NumberOfSubmitters = rolesHelper.UsersInRole("Submitter").Count();

            projectsDashboardVM.TicketsAssigned = db.Tickets.Where(t => t.Status.Name == "Assigned").Count();
            projectsDashboardVM.TicketsOpen = db.Tickets.Where(t => t.Status.Name == "Open").Count();
            projectsDashboardVM.TicketsReopened = db.Tickets.Where(t => t.Status.Name == "Reopened").Count();
            projectsDashboardVM.TicketsResolved = db.Tickets.Where(t => t.Status.Name == "Resolved").Count();

            projectsDashboardVM.TicketSoftware = db.Tickets.Where(t => t.TicketType.Name == "Software").Count();
            projectsDashboardVM.TicketHardware = db.Tickets.Where(t => t.TicketType.Name == "Hardware").Count();
            projectsDashboardVM.TicketUi = db.Tickets.Where(t => t.TicketType.Name == "UI").Count();
            projectsDashboardVM.TicketDefect = db.Tickets.Where(t => t.TicketType.Name == "Defect").Count();
            projectsDashboardVM.TicketOther = db.Tickets.Where(t => t.TicketType.Name == "Other").Count();

            projectsDashboardVM.TicketCritical = db.Tickets.Where(t => t.Priority.Name == "Critical").Count();
            projectsDashboardVM.TicketHigh = db.Tickets.Where(t => t.Priority.Name == "High").Count();
            projectsDashboardVM.TicketLow = db.Tickets.Where(t => t.Priority.Name == "Low").Count();
            projectsDashboardVM.TicketOnHold = db.Tickets.Where(t => t.Priority.Name == "On Hold").Count();

            projectsDashboardVM.RecentTickets = db.Tickets.OrderByDescending(t => t.Updated).Take(5).ToList();
            

            return View(projectsDashboardVM);

        }
        [Authorize]
        public PartialViewResult _LoginPartial()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var role = rolesHelper.ListUserRoles(userId).FirstOrDefault();
            
            var userData = new ProfileInfo();
            userData.FullName = user.FullName;
            userData.RoleName = role;
            userData.AvatarPath = user.AvatarPath;
            userData.Email = user.Email;

            return PartialView(userData);
        }

        public ActionResult Users()
        {
            var userIndexVM = new UserIndexVM();
            var users = db.Users.ToList();

            foreach(var user in users)
            {
                userIndexVM.Users.Add(new UserInfoVM
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    RoleName = rolesHelper.ListUserRoles(user.Id).FirstOrDefault(),
                    Email = user.Email,
                    NumberOfTickets = user.Projects.SelectMany(t => t.Tickets).Count()
                });
            }


            return View(userIndexVM);
        }

        public ActionResult Error()
        {
            return View("Shared/Errors");
        }
    }
}