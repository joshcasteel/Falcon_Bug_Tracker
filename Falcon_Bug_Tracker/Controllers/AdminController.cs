using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Falcon_Bug_Tracker.Helpers;
using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.ViewModel;

namespace Falcon_Bug_Tracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper userRoleHelper = new UserRolesHelper();
        
        // GET: Admin
        public ActionResult ManageRoles()
        {
            //combine user and role info into a view model
            var viewData = new List<UserInfoVM>();
            foreach (var user in db.Users.ToList())
            {
                viewData.Add(new UserInfoVM
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    RoleName = userRoleHelper.ListUserRoles(user.Id).FirstOrDefault()
                });
            }

            ViewBag.UserIds = new MultiSelectList(db.Users, "Id", "Email");
            ViewBag.RoleName = new SelectList(db.Roles, "Name", "Name");

            return View(viewData);
        }

        public ActionResult AddUserToRole(List<string> userIds, string roleName)
        {
            //go through each user selected and add to the selected role
            foreach(var userId in userIds)
            {
                //check if the user is currently in a role. if so, remove it and add new role.
                var currentRole = userRoleHelper.ListUserRoles(userId);
                if(currentRole.Count > 0)
                {
                    userRoleHelper.RemoveUserFromRole(userId, currentRole.FirstOrDefault());
                    userRoleHelper.AddUserToRole(userId, roleName);
                    
                }
                else
                {
                    userRoleHelper.AddUserToRole(userId, roleName);
                }
            }
            
            return RedirectToAction("ManageRoles");
        }


        public ActionResult RemoveUserFromRole(string userId, string roleName)
        {
            //check if user has role to remove
            if (roleName != null)
            {
                userRoleHelper.RemoveUserFromRole(userId, roleName);
            }
            
            return RedirectToAction("ManageRoles");
        }
    }
}