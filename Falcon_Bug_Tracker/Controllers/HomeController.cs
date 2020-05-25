using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.Helpers;
using Falcon_Bug_Tracker.ViewModel;

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
                return RedirectToAction("Dashboard");
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
            
            return View(projectsDashboardVM);

        }

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

    }
}