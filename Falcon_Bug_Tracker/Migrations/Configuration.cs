namespace Falcon_Bug_Tracker.Migrations
{
    using Falcon_Bug_Tracker.Helpers;
    using Falcon_Bug_Tracker.Models;
    using Microsoft.Ajax.Utilities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Falcon_Bug_Tracker.Models.ApplicationDbContext>
    {
        UserRolesHelper rolehelper = new UserRolesHelper();
        ProjectsHelper projHelper = new ProjectsHelper();
        
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Falcon_Bug_Tracker.Models.ApplicationDbContext context)
        {
            #region Role Creation
            var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            #endregion
            #region User Creation
            var userManager = new UserManager<ApplicationUser>(
               new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "joshcasteelz@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "joshcasteelz@gmail.com",
                    Email = "joshcasteelz@gmail.com",
                    FirstName = "Josh",
                    LastName = "Casteel",
                    AvatarPath = "/Images/Avatars/avatar_default.png",
                    EmailConfirmed = true
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "jcdemoadmin1@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemoadmin1@mailinator.com",
                    Email = "jcdemoadmin1@mailinator.com",
                    FirstName = "Jon",
                    LastName = "Snow",
                    AvatarPath = "/Images/Avatars/avatar_default.png",
                    EmailConfirmed = true
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "jcdemopm1@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemopm1@mailinator.com",
                    Email = "jcdemopm1@mailinator.com",
                    FirstName = "John",
                    LastName = "Locke",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "jcdemopm2@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemopm2@mailinator.com",
                    Email = "jcdemopm2@mailinator.com",
                    FirstName = "Jack",
                    LastName = "Shepard",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "jcdemodev1@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemodev1@mailinator.com",
                    Email = "jcdemodev1@mailinator.com",
                    FirstName = "Jake",
                    LastName = "Sully",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "jcdemodev2@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemodev2@mailinator.com",
                    Email = "jcdemodev2@mailinator.com",
                    FirstName = "Kate",
                    LastName = "Austen",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "jcdemodev3@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemodev3@mailinator.com",
                    Email = "jcdemodev3@mailinator.com",
                    FirstName = "James",
                    LastName = "Ford",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "jcdemodev4@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemodev4@mailinator.com",
                    Email = "jcdemodev4@mailinator.com",
                    FirstName = "Ben",
                    LastName = "Linus",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "jcdemodev5@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemodev5@mailinator.com",
                    Email = "jcdemodev5@mailinator.com",
                    FirstName = "Juliet",
                    LastName = "Burke",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "jcdemosub1@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemosub1@mailinator.com",
                    Email = "jcdemosub1@mailinator.com",
                    FirstName = "Karen",
                    LastName = "Blonde",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "jcdemosub2@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemosub2@mailinator.com",
                    Email = "jcdemosub2@mailinator.com",
                    FirstName = "Arya",
                    LastName = "Stark",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "jcdemosub3@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemosub3@mailinator.com",
                    Email = "jcdemosub3@mailinator.com",
                    FirstName = "Hugo",
                    LastName = "Reyes",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "jcdemosub4@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemosub4@mailinator.com",
                    Email = "jcdemosub4@mailinator.com",
                    FirstName = "Sayid",
                    LastName = "Jarrah",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "jcdemosub5@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcdemosub5@mailinator.com",
                    Email = "jcdemosub5@mailinator.com",
                    FirstName = "Claire",
                    LastName = "Littleton",
                    AvatarPath = "/Images/Avatars/avatar_default.png"
                }, "Abc&123!");
            }
            var AdminId = userManager.FindByEmail("joshcasteelz@gmail.com").Id;
            userManager.AddToRole(AdminId, "Admin");

            var AdminId2 = userManager.FindByEmail("jcdemoadmin1@mailinator.com").Id;
            userManager.AddToRole(AdminId2, "Admin");

            var PMId1 = userManager.FindByEmail("jcdemopm1@mailinator.com").Id;
            userManager.AddToRole(PMId1, "ProjectManager");

            var PMId2 = userManager.FindByEmail("jcdemopm2@mailinator.com").Id;
            userManager.AddToRole(PMId2, "ProjectManager");

            var DevId1 = userManager.FindByEmail("jcdemodev1@mailinator.com").Id;
            userManager.AddToRole(DevId1, "Developer");

            var DevId2 = userManager.FindByEmail("jcdemodev2@mailinator.com").Id;
            userManager.AddToRole(DevId2, "Developer");

            var DevId3 = userManager.FindByEmail("jcdemodev3@mailinator.com").Id;
            userManager.AddToRole(DevId3, "Developer");

            var DevId4 = userManager.FindByEmail("jcdemodev4@mailinator.com").Id;
            userManager.AddToRole(DevId4, "Developer");

            var DevId5 = userManager.FindByEmail("jcdemodev5@mailinator.com").Id;
            userManager.AddToRole(DevId5, "Developer");

            var SubId1 = userManager.FindByEmail("jcdemosub1@mailinator.com").Id;
            userManager.AddToRole(SubId1, "Submitter");

            var SubId2 = userManager.FindByEmail("jcdemosub2@mailinator.com").Id;
            userManager.AddToRole(SubId2, "Submitter");

            var SubId3 = userManager.FindByEmail("jcdemosub3@mailinator.com").Id;
            userManager.AddToRole(SubId3, "Submitter");

            var SubId4 = userManager.FindByEmail("jcdemosub4@mailinator.com").Id;
            userManager.AddToRole(SubId4, "Submitter");

            var SubId5 = userManager.FindByEmail("jcdemosub5@mailinator.com").Id;
            userManager.AddToRole(SubId5, "Submitter");

            #endregion
            #region Tickets Properties
            context.TicketTypes.AddOrUpdate(
                t => t.Name,
                new TicketType { Name = "Software" },
                new TicketType { Name = "Hardware" },
                new TicketType { Name = "UI" },
                new TicketType { Name = "Defect" },
                new TicketType { Name = "Other" }
                );
            
            context.TicketPriorities.AddOrUpdate(
                t => t.Name,
                new TicketPriority { Name = "Critical" },
                new TicketPriority { Name = "High" },
                new TicketPriority { Name = "Low" },
                new TicketPriority { Name = "On Hold" }
                );

            context.TicketStatuses.AddOrUpdate(
                t => t.Name,
                new TicketStatus { Name = "Open" },
                new TicketStatus { Name = "Assigned" },
                new TicketStatus { Name = "Resolved" },
                new TicketStatus { Name = "Reopened" },
                new TicketStatus { Name = "Archived" }
                );
            #endregion
            #region Project Creation
            context.Projects.AddOrUpdate(
                p => p.Name,
                new Project() { Name = "First Demo", Description = "The first demo project seed", Created = DateTime.Now.AddDays(-3) },
                new Project() { Name = "Second Demo", Description = "The second demo project seed", Created = DateTime.Now.AddDays(-7) },
                new Project() { Name = "Third Demo", Description = "The third demo project seed", Created = DateTime.Now.AddDays(-15) },
                new Project() { Name = "Fourth Demo", Description = "The fourth demo project seed", Created = DateTime.Now.AddDays(-45) },
                new Project() { Name = "Fifth Demo", Description = "The fifth demo project seed", Created = DateTime.Now.AddDays(-60) }
                );
            context.SaveChanges();
            #endregion

            
            Random rand = new Random();
            var ticketPriorityId = context.TicketPriorities.Select(t => t.Id).ToList();
            var ticketStatusId = context.TicketStatuses.FirstOrDefault(t => t.Name == "Open").Id;
            var ticketTypeId = context.TicketTypes.Select(t => t.Id).ToList();
            var projectId = context.Projects.Select(p => p.Id).ToList();
            List<string> submitters = rolehelper.UsersInRole("Submitter").Select(u => u.Id).ToList();
            for (var i = 1; i <= 20; i++)
            {
                int randProj = context.Projects.Find(rand.Next(1, (context.Projects.Count() + 1))).Id;
                string randSubmitter = submitters[rand.Next(0, submitters.Count())];
                if(!projHelper.IsUserOnProject(randSubmitter, randProj))
                {
                    projHelper.AddUserToProject(randSubmitter, randProj);
                }
                
                context.Tickets.AddOrUpdate(
                    t => t.Title,
                        new Ticket() 
                        { 
                            Title = $"Seeded Issue {i}",
                            Description = $"Description for Project {randProj}",
                            Created = DateTime.Now.AddDays(rand.Next(0, 20)),
                            SubmitterId = randSubmitter,
                            ProjectId = randProj,
                            TicketPriorityId = ticketPriorityId[rand.Next(0, ticketPriorityId.Count())],
                            TicketTypeId = ticketTypeId[rand.Next(0, ticketTypeId.Count())],
                            TicketStatusId = ticketStatusId 
                        });
            }
        }
    }
}
