using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Falcon_Bug_Tracker.Helpers
{
    public class UserProjectAssignmentHelper
    {
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();

        public ICollection<ApplicationUser> UsersOnProjectInRole(int projectId, string roleName)
        {
            var users = new List<ApplicationUser>();

            var projUsers = projHelper.UsersOnProject(projectId);
            foreach (var user in projUsers)
            {
                if (roleHelper.ListUserRoles(user.Id).FirstOrDefault() == roleName)
                {
                    users.Add(user);
                }
            }
            return users;
        }
    }
}