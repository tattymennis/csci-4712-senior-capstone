using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalManager.Data
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User { }

    public partial class UserMetadata
    {
        [Key]
        public int UID { get; set; }

        [Required(ErrorMessage = "Please enter a valid email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Title")]
        [StringLength(64, ErrorMessage = "Title must be between 1-64 characters.", MinimumLength = 1)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        [StringLength(16, ErrorMessage = "Role must be either Employee, Supervisor, or Administrator.", MinimumLength = 1)]
        public string Role { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First name")]
        [StringLength(50, ErrorMessage = "Please enter a valid name.", MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last name")]
        [StringLength(50, ErrorMessage = "Please enter a valid name.", MinimumLength = 1)]
        public string LastName { get; set; }

        [Required]
        public int DID { get; set; }

        [Required]
        public bool Active { get; set; }

        public Nullable<int> SUID { get; set; }

        [Required]
        public string Username { get; set; }
    }

    [MetadataType(typeof(DepartmentMetadata))]
    public partial class Department { }

    public class DepartmentMetadata
    {

    }

    [MetadataType(typeof(GoalMetadata))]
    public partial class Goal
    {
        public Goal(string title, string categoryName, string status, int progress)
        {
            Title = title;
            Category = categoryName;
            Status = status;
            Progress = progress; // type double vs int
        }

        public Goal(string title, string categoryName, string status, string desc, int progress)
        {
            Title = title;
            Category = categoryName;
            Status = status;
            Progress = progress; // type double vs int
            Description = desc;
        }
    }

    public class GoalMetadata
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public double Progress { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }

    [MetadataType(typeof(UpdateMetadata))]
    public partial class Update
    {
        public Update() { }
        public Update(string subject, string notes, int progress, DateTime time)
        {
            Subject = subject;
            Notes = notes;
            Progress = progress;
            Time = time;
        }
    }
    public class UpdateMetadata
    {
        [Key]
        public int UpID { get; set; }
        public int GID { get; set; }
        public int Progress { get; set; }
        public string Notes { get; set; }
        public string Subject { get; set; }
        public Nullable<System.DateTime> Time { get; set; }

        public virtual Goal Goal { get; set; }
    }

    [MetadataType(typeof(QuarterMetadata))]
    public partial class Quarter { }

    public class QuarterMetadata { }

    [MetadataType(typeof(CategoryMetadata))]
    public partial class Category { }

    public class CategoryMetadata { }

    // Helper class to save clean, database data to Session
    public class UserSessionData
    {
        public int UID { get; set; }

        public int DID { get; set; }

        public List<Goal> Goals { get; set; }

        public Dictionary<int,List<Update>> GoalUpdatesTable { get; set; }

        public string Username { get; set; }

        public string Role { get; set; }
    }
}
