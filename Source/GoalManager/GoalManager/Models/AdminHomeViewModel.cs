using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;

namespace GoalManager.Models
{
    public class AdminHomeViewModel
    {
        public List<User> Employees = new List<User>();
        public List<Department> Departments = new List<Department>();
    }
}
