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
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                var assignUsersVM = new AssignUsersVM();
                var users = db.Users.ToList();
                
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
                
                var developers = userHelper.UsersInRole("Developer");
                var submitters = userHelper.UsersInRole("Submitter");

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
            if ((developerSelectList == null && submitterSelectList == null) || projectSelectList == null)
                return RedirectToAction("ManageProjectAssignment");

            var userIds = new List<string>();
            if(developerSelectList != null)
            {
                userIds.AddRange(developerSelectList);
            }
            if(submitterSelectList != null)
            {
                userIds.AddRange(submitterSelectList);
            }
           

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
            projHelper.RemoveUserFromProject(userId, projectId);
            return RedirectToAction("ManageProjectAssignments");
        }

        public ActionResult AssignUsers(int projectId)
        {
            var addUsers = new AddUsersToProjectVM();

            var project = db.Projects.Find(projectId);
            addUsers.Project = project;
            
            addUsers.ProjectManagers = new SelectList(userHelper.UsersInRole("ProjectManager"), "Id", "FullName");
            addUsers.Developers = new MultiSelectList(userHelper.UsersInRole("Developer"), "Id", "FullName");
            addUsers.Submitters = new MultiSelectList(userHelper.UsersInRole("Submitter"), "Id", "FullName");

            if(project.ProjectManagerId != null)
            {
                addUsers.CurrentProjectManager = db.Users.Find(project.ProjectManagerId).FullName;
            }
            
            var projectUsers = projHelper.UsersOnProject(projectId).ToList();
            foreach(var user in projectUsers)
            {
                var role = userHelper.ListUserRoles(user.Id).FirstOrDefault();
                if(role == "Developer")
                {
                    addUsers.CurrentDevelopers.Add(user.FullName);
                }
                if(role == "Submitter")
                {
                    addUsers.CurrentSubmitters.Add(user.FullName);
                }
            }
            

            return View(addUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignUsers(int projectId, AddUsersToProjectVM model)
        {
                       
            if(User.IsInRole("Admin"))
            {
                var project = db.Projects.Find(projectId);
                project.ProjectManagerId = model.ProjectManagerId;
                db.SaveChanges();
            }
            else
            {
                foreach (var user in assignHelper.UsersOnProjectInRole(projectId, "Submitter"))
                {
                    projHelper.RemoveUserFromProject(user.Id, projectId);
                }

                if(model.SubmitterIds != null)
                {
                    foreach(var submitter in model.SubmitterIds)
                    {
                        projHelper.AddUserToProject(submitter, projectId);
                    }
                }

                if(model.DeveloperIds != null)
                {
                    foreach(var developer in model.DeveloperIds)
                    {
                        projHelper.AddUserToProject(developer, projectId);
                    }
                }



            }

            return RedirectToAction("Index", "Projects");
        }


        // GET: Projects
        public ActionResult Index()
        {
            ProjectIndexVM projectIndex = new ProjectIndexVM();
            var userId = User.Identity.GetUserId();

            

            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                projectIndex.AllProjects = db.Projects.ToList();
                foreach (var project in projectIndex.AllProjects)
                {
                    if (project.ProjectManagerId != null)
                    {
                        string fullName = db.Users.Find(project.ProjectManagerId).FullName;
                        project.ProjectManagerId = fullName;
                    }
                }

                projectIndex.MyProjects = db.Projects.Where(p => p.ProjectManagerId == userId).ToList();
                

                return View(projectIndex);
            }


            projectIndex.MyProjects = projHelper.ListUserProjects(userId).ToList();
            foreach (var project in projectIndex.MyProjects)
            {
                if (project.ProjectManagerId != null)
                {
                    string fullName = db.Users.Find(project.ProjectManagerId).FullName;
                    project.ProjectManagerId = fullName;
                }
            }

            return View(projectIndex);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
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
            var projectDetailsVM = new ProjectDetailsVM();
            projectDetailsVM = new ProjectDetailsVM
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                ProjectDescription = project.Description,
                Created = project.Created,
                Updated = project.Updated,
            };
            if (project.ProjectManagerId != null)
            {
                projectDetailsVM.ProjectManager = db.Users.Find(project.ProjectManagerId).FullName;
            }
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            if(User.IsInRole("Submitter") || User.IsInRole("Developer"))
            {
                TempData["Alert"] = "You are not authorized to edit projects";
                return RedirectToAction("Index", "Projects");
            }
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
            if (ModelState.IsValid)
            {
                project.Name = model.Project.Name;
                project.Description = model.Project.Description;
                project.ProjectManagerId = projectManagerId;
                project.IsArchived = model.Project.IsArchived;
                project.Updated = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
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
