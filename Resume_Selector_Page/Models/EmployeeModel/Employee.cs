using Microsoft.AspNetCore.Identity;

namespace Resume_Selector_Page.Models.EmployeeModel
{
    public class Employee : IdentityUser
    {
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Field { get; set; }
        public string Skills { get; set; }
        public string Experience { get; set; }
        public string Location { get; set; }
        public string Summary { get; set; }
        public string? ResumeFileName { get; set; }
        public string? ResumeFilePath { get; set; }
    }
}
