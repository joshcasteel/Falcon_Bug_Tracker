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
        public List<ProjectDetailsVM> AllProjects { get; set; }
        public List<ProjectDetailsVM> MyProjects { get; set; }

        public ProjectIndexVM()
        {
            AllProjects = new List<ProjectDetailsVM>();
            MyProjects = new List<ProjectDetailsVM>();
        }
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

    public class ProjectDetailsVM
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectManager { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public List<UserInfoVM> Users { get; set; }
        public int TicketCount { get; set; }

        public ProjectDetailsVM()
        {
            Users = new List<UserInfoVM>();
        }
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

    public class ProjectDashboardVM
    {
        public int NumberOfProjects { get; set; }
        public int NumberOfTickets { get; set; }
        public int NumberOfProjectManagers { get; set; }
        public int NumberOfDevelopers { get; set; }
        public int NumberOfSubmitters { get; set; }

        public int TicketsOpen { get; set; }
        public int TicketsAssigned { get; set; }
        public int TicketsResolved { get; set; }
        public int TicketsReopened { get; set; }

        public int TicketSoftware { get; set; }
        public int TicketHardware { get; set; }
        public int TicketUi { get; set; }
        public int TicketDefect { get; set; }
        public int TicketOther { get; set; }

        public int TicketCritical { get; set; }
        public int TicketHigh { get; set; }
        public int TicketLow { get; set; }
        public int TicketOnHold { get; set; }
        public IEnumerable<Ticket> RecentTickets { get; set; }


    }
}