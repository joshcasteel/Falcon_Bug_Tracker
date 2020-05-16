using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class ProjectEditVM
    {
        [Display(Name = "Project Manager")]
        public IEnumerable<SelectListItem> ProjectManagers { get; set; }
        public string ProjectManagerId { get; set; }
        public Project Project { get; set; }
    }

    public class ProjectIndexVM
    {
        public List<Project> AllProjects { get; set; }
                  
        
        //public int Id { get; set; }
        //public string Name { get; set; }
        //public string Description { get; set; }
        //public string ProjectManager { get; set; }
        //public DateTime Created { get; set; }
        //public DateTime? Updated { get; set; }

    }
}