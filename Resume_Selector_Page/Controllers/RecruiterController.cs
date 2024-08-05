using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Resume_Selector_Page.Data;

namespace Resume_Selector_Page.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruiterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileProvider fileProvider1;
        private const string UploadsFolder = "Uploads";


        public RecruiterController(AppDbContext context, IWebHostEnvironment environment, IFileProvider fileProvider1)
        {
            this._context = context;
            this._environment = environment;
            this.fileProvider1 = fileProvider1;
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("recruiters")]
        public async Task<IActionResult> GetAllRecruiters()
        {
            var recruiters = await _context.Recruiters.Select(r => new
            {
                Id = r.Id,
                Name = r.Name,  
                Company = r.Company,
                PhoneNumber = r.PhoneNumber,
                Email = r.Email,
            }).ToListAsync();

            return Ok(recruiters);
        }
        
        
        
        //[Authorize(Roles = "Admin")]
        [HttpDelete("recruiters/{id}")]
        public async Task<IActionResult> RemoveRecruiter(string id)
        {
            var recruiter = await _context.Recruiters.FindAsync(id);
            if(recruiter == null)
            {
                return NotFound();
            }
            _context.Recruiters.Remove(recruiter);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
