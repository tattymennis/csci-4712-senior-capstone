using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoalManager.Models
{
    public class ModifyDepartmentViewModel
    {

        public int DID { get; set; }
        public string Name { get; set; }
        
        public string Location { get; set; }
        public string Description { get; set; }
        public int IDRef { get; set; }
    }
}