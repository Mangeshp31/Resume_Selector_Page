using Resume_Selector_Page.Migrations;
using Resume_Selector_Page.Models.EmployeeModel;

namespace Resume_Selector_Page.Data
{
    public class CommonRole
    {
        public Employee Employee { get; set; }
        public Recruiter Recruiter { get; set; }
        public Admin Admin { get; set; }
    }
}
