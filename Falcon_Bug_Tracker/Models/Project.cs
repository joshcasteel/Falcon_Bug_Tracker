﻿using System;
using System.Collections.Generic;


namespace Falcon_Bug_Tracker.Models
{
    public class Project
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectManagerId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsArchived { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public Project()
        {
            Users = new HashSet<ApplicationUser>();
            Tickets = new HashSet<Ticket>();
        }
    }
}