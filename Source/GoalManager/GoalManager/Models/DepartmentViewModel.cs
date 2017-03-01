using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;

namespace GoalManager.Models
{
    public class DepartmentViewModel
    {
        public Department Department = new Department();

        public List<User> Supervisors = new List<User>();
    }
}