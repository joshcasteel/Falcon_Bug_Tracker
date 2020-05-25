using Falcon_Bug_Tracker.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class UserInfoVM
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public List<string> ProjectNames { get; set; }
        public List<int> ProjectIds { get; set; }
    }

    public class ProfileInfo
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; }

        public string Email { get; set; }

        public string AvatarPath { get; set; }

    }

    public class AssignUsersVM
    {
        public AssignUsersVM()
        {
            Users = new List<UserInfoVM>();
        }
        public List<UserInfoVM> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }

        public IEnumerable<SelectListItem> DeveloperSelectList { get; set; }
        public IEnumerable<SelectListItem> SubmitterSelectList { get; set; }
        public IEnumerable<SelectListItem> ProjectSelectList { get; set; }
    }
}