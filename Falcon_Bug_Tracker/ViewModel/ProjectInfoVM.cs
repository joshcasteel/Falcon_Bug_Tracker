using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class ProjectIndexVM
    {
        public List<Project> AllProjects { get; set; }
        public List<Project> MyProjects { get; set; }

    }

    public class ProjectCreateVM
    {
        [Display(Name = "Project Manager")]
        public IEnumerable<SelectListItem> ProjectManagers { get; set; }
        public string ProjectManagerId { get; set; }
        public Project Project { get; set; }
    }

    public class ProjectEditVM
    {
        [Display(Name = "Project Manager")]
        public IEnumerable<SelectListItem> ProjectManagers { get; set; }
        public string ProjectManagerId { get; set; }
        public Project Project { get; set; }
    }

    
}