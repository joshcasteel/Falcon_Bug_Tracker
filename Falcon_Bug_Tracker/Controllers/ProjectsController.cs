using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private UserRolesHelper userHelper = new UserRolesHelper();

        public ActionResult ManageProjectAssignments()
        {
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                var customUserDataList = new List<UserInfoVM>();
                var users = db.Users.ToList();


                ViewBag.UserIds = new MultiSelectList(db.Users, "Id", "FullName");
                ViewBag.ProjectIds = new MultiSelectList(db.Projects, "Id", "Name");

                foreach (var user in users)
                {
                    customUserDataList.Add(new UserInfoVM
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        ProjectNames = projHelper.ListUserProjects(user.Id).Select(p => p.Name).ToList(),
                        ProjectIds = projHelper.ListUserProjects(user.Id).Select(p => p.Id).ToList()
                    });
                }

                return View(customUserDataList);
            }
            TempData["Alert"] = "You are not authorized to assign projects";
            return RedirectToAction("Index", "Projects");
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProjectAssignments(List<string> userIds, List<int> projectIds)
        {
            if (userIds == null || projectIds == null)
                return RedirectToAction("ManageProjectAssignment");

            foreach(var userId in userIds)
            {
                foreach(var projectId in projectIds)
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

            return View(project);
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
                return RedirectToAction("Index");
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
            projectEdit.ProjectManagers = new SelectList(pmUsers, "Id", "FullName");

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
