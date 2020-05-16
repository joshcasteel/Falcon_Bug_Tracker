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
                return View("Dashboard");
            }
            return View();
        }
        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }

        public PartialViewResult _LoginPartial()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var role = rolesHelper.ListUserRoles(userId).FirstOrDefault();
            UserInfoVM userData = new UserInfoVM();
            userData.FullName = user.FullName;
            userData.RoleName = role;

            return PartialView(userData);
        }

    }
}