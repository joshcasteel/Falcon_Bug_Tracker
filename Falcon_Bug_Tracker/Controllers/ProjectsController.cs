using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Falcon_Bug_Tracker.Helpers;
using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.ViewModel;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;



namespace Falcon_Bug_Tracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private UserRolesHelper userHelper = new UserRolesHelper();
        private UserProjectAssignmentHelper assignHelper = new UserProjectAssignmentHelper();

        public ActionResult ManageProjectAssignments()
        {
            //Checks if user is in appropriate role, if not they're redirected with warning message.
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                //create an instance of my view model
                var assignUsersVM = new AssignUsersVM();
                //assign all users to a list variable
                var users = db.Users.ToList();
                
                //store user, role and project info in the view model
                foreach (var user in users)
                {
                    assignUsersVM.Users.Add(new UserInfoVM
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        RoleName = userHelper.ListUserRoles(user.Id).FirstOrDefault(),
                        Email = user.Email,
                        Projects = projHelper.ListUserProjects(user.Id).ToList()
                    });
                }
                
                //breakout users by role
                var developers = userHelper.UsersInRole("Developer");
                var submitters = userHelper.UsersInRole("Submitter");

                //create the selectlists for the view
                assignUsersVM.DeveloperSelectList = new MultiSelectList(developers, "Id", "FullName");
                assignUsersVM.SubmitterSelectList = new MultiSelectList(submitters, "Id", "FullName");
                assignUsersVM.ProjectSelectList = new MultiSelectList(db.Projects, "Id", "Name");
                

                return View(assignUsersVM);
            }
            TempData["Alert"] = "You are not authorized to assign projects";
            return RedirectToAction("Index", "Projects");
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProjectAssignments(List<string> developerSelectList, List<string> submitterSelectList, List<int> projectSelectList)
        {
            //Check to see if at least one user and one project have been selected
            if ((developerSelectList == null && submitterSelectList == null) || projectSelectList == null)
            {
                TempData["Alert"] = "Please select at least one user and project";
                return RedirectToAction("ManageProjectAssignments");
            }
            
            //Container variable to combine the two selectlists of users
            var userIds = new List<string>();

            //Check to see if developers were selected. If so, add to container variable.
            if(developerSelectList != null)
            {
                userIds.AddRange(developerSelectList);
            }
            //Check to see if submitters were selected. If so, add to container variable.
            if (submitterSelectList != null)
            {
                userIds.AddRange(submitterSelectList);
            }
            
            //Go through each users selected and add them to each project selected
            foreach(var userId in userIds)
            {
                foreach(var projectId in projectSelectList)
                {
                    
                    projHelper.AddUserToProject(userId, projectId);
                }
            }

            return RedirectToAction("ManageProjectAssignments");
        }

        public ActionResult RemoveUserFromProject(string userId, int projectId)
        {
            //checks if user is in appropriate role.
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                projHelper.RemoveUserFromProject(userId, projectId);
                return RedirectToAction("ManageProjectAssignments");
            }
            TempData["Alert"] = "You are not authorized to change user assignments";
            return RedirectToAction("Index", "Projects");
        }


        // GET: Projects
        public ActionResult Index()
        {
            //creates instance of viewmodel 
            ProjectIndexVM projectIndex = new ProjectIndexVM();
            var userId = User.Identity.GetUserId();

            
            //project managers and admins see all projects and projects assigned to them
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                
                foreach (var project in db.Projects.ToList())
                {
                    projectIndex.AllProjects.Add(new ProjectDetailsVM
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        ProjectDescription = project.Description,
                        ProjectManager = project.ProjectManagerId.Any() ? db.Users.Find(project.ProjectManagerId).FullName : "",
                        Created = project.Created,
                        Updated = project.Updated,
                        TicketCount = project.Tickets.Count()
                    });
                }

                var myProjects =  db.Projects.Where(p => p.ProjectManagerId == userId).ToList();
                foreach (var project in myProjects)
                {
                    projectIndex.MyProjects.Add(new ProjectDetailsVM
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        ProjectDescription = project.Description,
                        ProjectManager = project.ProjectManagerId.Any() ? db.Users.Find(project.ProjectManagerId).FullName : "",
                        Created = project.Created,
                        Updated = project.Updated,
                        TicketCount = project.Tickets.Count()
                    });
                }
                
                return View(projectIndex);
            }

            //submitters and developers only see projects assigned to them
            foreach (var project in projHelper.ListUserProjects(userId).ToList())
            {
                projectIndex.MyProjects.Add(new ProjectDetailsVM
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    ProjectDescription = project.Description,
                    ProjectManager = project.ProjectManagerId.Any() ? db.Users.Find(project.ProjectManagerId).FullName : "",
                    Created = project.Created,
                    Updated = project.Updated,
                    TicketCount = project.Tickets.Count()
                });
            }
        

            return View(projectIndex);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            //if the id is missing, redirect and alert the user
            if (id == null)
            {
                TempData["Alert"] = "Please provide a project id";
                return RedirectToAction("Index");
            }

            //create an instance of the specified project
            Project project = db.Projects.Find(id);
            //ensure it exists
            if (project == null)
            {
                TempData["Alert"] = "Project not found";
                return RedirectToAction("Index");
            }

            //instantiate view model
            var projectDetailsVM = new ProjectDetailsVM();
            projectDetailsVM = new ProjectDetailsVM
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                ProjectDescription = project.Description,
                Created = project.Created,
                Updated = project.Updated,
                ProjectManager = project.ProjectManagerId.Any() ? db.Users.Find(project.ProjectManagerId).FullName : ""
            };

            //List users on project
            var projUsers = projHelper.UsersOnProject(project.Id);
            foreach(var user in projUsers)
            {
                projectDetailsVM.Users.Add(new UserInfoVM
                {
                    FullName = user.FullName,
                    RoleName = userHelper.ListUserRoles(user.Id).FirstOrDefault()
                });
            }

            return View(projectDetailsVM);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                ProjectCreateVM projectCreate = new ProjectCreateVM();
                var pmUsers = userHelper.UsersInRole("ProjectManager");
                projectCreate.ProjectManagers = new SelectList(pmUsers, "Id", "FullName");


                return View(projectCreate);
            }
            TempData["Alert"] = "You are not authorized to create projects";
            return RedirectToAction("Index", "Projects");
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectCreateVM projectCreate, string projectManagerId)
        {   
            if (ModelState.IsValid)
            {
                Project project = new Project();
                project.Name = projectCreate.Project.Name;
                project.Description = projectCreate.Project.Description;
                project.ProjectManagerId = projectManagerId;
                project.Created = DateTime.Now;
                
                db.Projects.Add(project);
                db.SaveChanges();
                var projId = project.Id;
                return RedirectToAction("Details", new { id = projId });
            }

            return View(projectCreate);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            //validation area
            if (id == null)
            {
                TempData["Alert"] = "Please provide a project id";
                return RedirectToAction("Index");
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                TempData["Alert"] = "Project not found";
                return RedirectToAction("Index");
            }
            if(User.IsInRole("Submitter") || User.IsInRole("Developer"))
            {
                TempData["Alert"] = "You are not authorized to edit projects";
                return RedirectToAction("Index", "Projects");
            }

            //instantiate, populate and pass the view model to the view
            var projectEdit = new ProjectEditVM();
            projectEdit.Project = project;
            var pmUsers= userHelper.UsersInRole("ProjectManager");
            projectEdit.ProjectManagers = new SelectList(pmUsers, "Id", "FullName", project.ProjectManagerId);

            return View(projectEdit);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectEditVM model, string projectManagerId)
        {
            Project project = db.Projects.Find(model.Project.Id);
            project.Name = model.Project.Name;
            project.Description = model.Project.Description;
            project.ProjectManagerId = projectManagerId;
            project.IsArchived = model.Project.IsArchived;
            project.Updated = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
