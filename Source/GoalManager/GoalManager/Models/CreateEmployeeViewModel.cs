using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;
using System.ComponentModel.DataAnnotations;

namespace GoalManager.Models
{
    public class CreateEmployeeViewModel
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Title { get; set; }

        public string Role { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int DID { get; set; }

        public bool Active { get; set; }

        public int SUID { get; set; }

        public string Username { get; set; }

        public User Employee = new User();
        public List<Department> Departments = new List<Department>();
    }
}