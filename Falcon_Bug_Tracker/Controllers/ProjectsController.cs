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
using Falcon_Bug_Tracker.Helpers;
using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.ViewModel;
using Microsoft.AspNet.Identity;



namespace Falcon_Bug_Tracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private UserRolesHelper userHelper = new UserRolesHelper();

        [Authorize(Roles = "ProjectManager,Admin")]
        public ActionResult ManageProjectAssignments()
        {
            var customUserDataList = new List<CustomUserData>();
            var users = db.Users.ToList();
            
            
            ViewBag.UserIds = new MultiSelectList(db.Users, "Id", "FullName");
            ViewBag.ProjectIds = new MultiSelectList(db.Projects, "Id", "Name");

            foreach(var user in users)
            {
                customUserDataList.Add(new CustomUserData
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
            return View(db.Projects.ToList());
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
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,ProjectManagerId,Created,Updated,IsArchived")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTime.Now;
                
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
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
            var projectInfo = new ProjectInfo();
            projectInfo.Project = project;
            var pmUsers= userHelper.UsersInRole("ProjectManager");
            projectInfo.ProjectManagers = new SelectList(pmUsers, "Id", "FullName");

            return View(projectInfo);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectInfo model, string projectManagerId)
        {
            Project project = db.Projects.Find(model.Project.Id);
            if (ModelState.IsValid)
            {
                project.Name = model.Project.Name;
                project.Description = model.Project.Description;
                project.ProjectManagerId = projectManagerId;
               
                project.Updated = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
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
