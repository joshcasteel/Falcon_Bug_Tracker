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

    public class AddUsersToProjectVM
    {
        public AddUsersToProjectVM()
        {
            CurrentDevelopers = new List<string>();
            CurrentSubmitters = new List<string>();
            SubmitterIds = new List<string>();
            DeveloperIds = new List<string>();
        }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public IEnumerable<SelectListItem> ProjectManagers { get; set; }
        public string ProjectManagerId { get; set; }
        public IEnumerable<SelectListItem> Developers { get; set; }
        public List<string> DeveloperIds { get; set; }
        public IEnumerable<SelectListItem> Submitters { get; set; }
        public List<string> SubmitterIds { get; set; }
        public string CurrentProjectManager { get; set; }
        public List<string> CurrentDevelopers { get; set; }
        public List<string> CurrentSubmitters { get; set; }
    }

    
}