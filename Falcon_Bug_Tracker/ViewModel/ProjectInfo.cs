using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class ProjectInfo
    {
        
        public IEnumerable<SelectListItem> ProjectManagers { get; set; }
        public string ProjectManagerId { get; set; }
        
        public Project Project { get; set; }

    }
}