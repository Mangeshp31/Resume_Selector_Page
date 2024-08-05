using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Resume_Selector_Page.Data;
using Resume_Selector_Page.Models.EmployeeModel;
using Resume_Selector_Page.Models.RecruiterModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Resume_Selector_Page.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public EmployeeController(UserManager<Employee> userManager, SignInManager<Employee> signInManager,
            IWebHostEnvironment environment, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] EmployeeRegistrationModel model)
        {
            var employee = new Employee
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                Contact = model.Contact,
                Field = model.Field,
                Skills = model.Skills,
                Experience = model.Experience,
                Location = model.Location,
                Summary = model.Summary
            };

            var result = await _userManager.CreateAsync(employee, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Employee registered successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpPost("upload-resume")]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByEmailAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var uploadPath = Path.Combine(_environment.WebRootPath, "Uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ResumeFileName = file.FileName;
            user.ResumeFilePath = filePath;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Resume uploaded successfully" });
            }

            return BadRequest(result.Errors);
        }

        //[Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByEmailAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Name = model.Name;
            user.Contact = model.Contact;
            user.Field = model.Field;
            user.Skills = model.Skills;
            user.Experience = model.Experience;
            user.Location = model.Location;
            user.Summary = model.Summary;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Employee details updated successfully" });
            }

            return BadRequest(result.Errors);
        }

        //[Authorize]
        [HttpGet("details")]
        public async Task<IActionResult> GetEmployeeDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByEmailAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var employeeDetails = new
            {
                user.Name,
                user.Contact,
                user.Field,
                user.Skills,
                user.Experience,
                user.Location,
                user.Summary,
                user.ResumeFileName,
                ResumeFileUrl = Url.Action(nameof(DownloadResume), new { user.ResumeFileName })
            };

            return Ok(employeeDetails);
        }

        //[Authorize]
        [HttpGet("download-resume")]
        public IActionResult DownloadResume(string fileName)
        {
            var uploadPath = Path.Combine(_environment.WebRootPath, "Uploads");
            var filePath = Path.Combine(uploadPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found");
            }

            var mimeType = fileName.EndsWith(".pdf") ? "application/pdf" :
                fileName.EndsWith(".doc") ? "application/msword" :
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            return PhysicalFile(filePath, mimeType, fileName);
        }
    }

    public class EmployeeRegistrationModel
    {
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Field { get; set; }
        public string Skills { get; set; }
        public string Experience { get; set; }
        public string Location { get; set; }
        public string Summary { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdateEmployeeModel
    {
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Field { get; set; }
        public string Skills { get; set; }
        public string Experience { get; set; }
        public string Location { get; set; }
        public string Summary { get; set; }
    }
}
