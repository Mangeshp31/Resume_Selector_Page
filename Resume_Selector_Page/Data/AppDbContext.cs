using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Resume_Selector_Page.Models;
using Resume_Selector_Page.Models.Admin;
using Resume_Selector_Page.Models.EmployeeModel;
using static Resume_Selector_Page.Data.AppDbContext;

namespace Resume_Selector_Page.Data
{
    //public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    //public     class AppDbContext : IdentityDbContext<Recruiter>
    public class AppDbContext : IdentityDbContext<Employee>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Resume> Resumes { get; set; }

        public DbSet<Recruiter> Recruiters { get; set; }
		public DbSet<Employee> Employees { get; set; }



		//changes for admin
		public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<DownloadedResume> DownloadedResumes { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        //public class ApplicationUser : IdentityUser
        //{

        //}
        //public class AdminUser : ApplicationUser
        //{

        //}
        //public class Recruiter : ApplicationUser
        //{

        //}
    }
}
