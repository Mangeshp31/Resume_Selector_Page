using Aspose.Words;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Resume_Selector_Page.Data;
using Resume_Selector_Page.Models;
using System;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using UglyToad.PdfPig;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Resume_Selector_Page.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileProvider fileProvider1;
        private const string UploadsFolder = "Uploads";


        public ResumesController(AppDbContext context, IWebHostEnvironment environment, IFileProvider fileProvider1)
        {
            this._context = context;
            this._environment = environment;
            this.fileProvider1 = fileProvider1;
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllProfile()
        {
            var resumes = await _context.Resumes.Select(r => new
            {
                Id = r.Id,
                FileName = r.FileName,
                PdfUrl = Url.Action(nameof(DownloadResume), "Resumes", new { id = r.Id }, Request.Scheme)
            }).ToListAsync();
            return Ok(resumes);
        }


        //[Authorize] right working
        //[HttpGet("download/{id}")]
        //public IActionResult DownloadResume(int id)
        //{
        //    var resume = _context.Resumes.FirstOrDefault(x => x.Id == id);
        //    if (resume == null)
        //    {
        //        return NotFound();
        //    }

        //    if (string.IsNullOrEmpty(resume.FileName))
        //    {
        //        return NotFound();
        //    }


        //    //var filePath = fileProvider1.GetDirectoryContents("");
        //    //return ;
        //    var filePath = Path.Combine(_environment.WebRootPath, resume.FileName);
        //    if (!System.IO.File.Exists(filePath))
        //    {

        //        return NotFound("File not found on server");
        //    }

        //    //Track Download

        //    var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (recruiterId == null)
        //    {
        //        return Unauthorized();
        //    }
        //    var downloadedResume = new DownloadedResume
        //    {
        //        RecruiterId = recruiterId,
        //        ResumeId = resume.Id,
        //        DownloadedAt = DateTime.UtcNow,
        //    };
        //    _context.DownloadedResumes.Add(downloadedResume);
        //    _context.SaveChanges();


        //    var mimeType = "application/pdf";
        //    return PhysicalFile(filePath, mimeType, resume.FileName);
        //}



        //[HttpGet("download/{id}")]working
        //public IActionResult DownloadResume(int id)
        //{
        //    var resume = _context.Resumes.Find(id);
        //    if (resume == null)
        //    {
        //        return NotFound();
        //    }
        //    var filePath = Path.Combine(_environment.WebRootPath,"uploads", resume.FileName);
        //    if(!System.IO.File.Exists(filePath))
        //    {
        //        return NotFound("file not found");
        //    }
        //    var mimeType = resume.FileType switch
        //    {
        //        ".pdf" => "application/pdf",
        //        ".doc" => "application/msword",
        //        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        //        _ => "application/octet-stream"
        //    };
        //    return PhysicalFile(filePath, mimeType, resume.FileName);
        //}


        
        [HttpGet("download/{id}")]
        public IActionResult DownloadResume(int id)
        {
            var resume = _context.Resumes.FirstOrDefault(x => x.Id == id);
            if (resume == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_environment.WebRootPath, resume.FilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on server.");
            }

            //var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (recruiterId == null)
            //{
            //    return Unauthorized();
            //}

            //var downloadedResume = new DownloadedResume
            //{
            //    RecruiterId = recruiterId,
            //    ResumeId = resume.Id,
            //    DownloadedAt = DateTime.UtcNow,
            //};
            //_context.DownloadedResumes.Add(downloadedResume);
            //_context.SaveChanges();

            var mimeType = GetMimeType(resume.FilePath);
            return PhysicalFile(filePath, mimeType, resume.FileName);
        }

        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream",
            };
        }


        //extra
        [HttpPost("downloadMultiple")]
        public IActionResult DownloadMultipleResumes([FromBody] List<int> ids)
        {
            var resumeUrls = _context.Resumes
                .Where(r => ids.Contains(r.Id))
                .Select(r => Url.Action("DownloadResume", new { id = r.Id,}))
                .ToList();
            if(resumeUrls == null || !resumeUrls.Any())
            {
                return NotFound("No resume found");
            }
            return Ok(resumeUrls);
        }





        [Authorize]
        [HttpGet("id")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(resume);
        }


        //working
        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadResume(IFormFile file)
        //{
        //    //if (file == null || file.Length ==0 )
        //    //{
        //    //    return BadRequest(" Invalid File Formate");
        //    //}

        //    ////for doc
        //    //var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        //    //var fileExtension = Path.GetExtension(file.FileName).ToLower();
        //    //if (!allowedExtensions.Contains(fileExtension))
        //    //{
        //    //    return BadRequest("Invalid file formate");
        //    //}
        //    //var fileName = Path.GetFileName(file.FileName);
        //    //var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
        //    //using (var stream = new FileStream(filePath, FileMode.Create))
        //    //{
        //    //    await file.CopyToAsync(stream);
        //    //}

        //    //string content;
        //    //if(fileExtension ==".pdf")
        //    //{
        //    //    content = ExtractTextFromPdf(filePath);
        //    //}
        //    //else
        //    //{
        //    //    content = ExtractTextFromWord(filePath);
        //    //}

        //    //var resume = new Resume
        //    //{
        //    //    FileName = fileName,
        //    //    FilePath = filePath,
        //    //    FileType = fileExtension,
        //    //    Content = content

        //    //};
        //    //_context.Resumes.Add(resume);
        //    //await _context.SaveChangesAsync();
        //    //return Ok(new {resume.Id, resume.FileName, resume.FileType});






        //    var basePath = Directory.GetCurrentDirectory();//Get the current working directory
        //    var uploadPath = Path.Combine(basePath, UploadsFolder);//combines baspath with Uploads folder

        //    //ensure directory exixt and create it
        //    if (!Directory.Exists(uploadPath))
        //    {
        //        Directory.CreateDirectory(uploadPath);
        //    }

        //    //combines the upload path with filename to create full path
        //    var filePath = Path.Combine("Uploads", file.FileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);//create a file stream to write the uploaded file to the specified path
        //    }


        //    string content = ExtractTextFromPdf(filePath);

        //    var resume = new Resume //ceate an object
        //    {
        //        FileName = filePath,
        //        FilePath = filePath,
        //        Content = content
        //    };

        //    _context.Resumes.Add(resume);
        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}

        //private string ExtractTextFromPdf(string filePath)
        //{
        //    using (var pdf = PdfDocument.Open(filePath)) //opens the pdf document using file path
        //    {
        //        var text = new StringBuilder();
        //        foreach(var page in pdf.GetPages())
        //        {
        //            text.Append(page.Text);
        //        }
        //        return text.ToString();
        //    }
        //}


        //[Authorize]



        [HttpPost("upload")]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            var basePath = Directory.GetCurrentDirectory();
            var uploadPath = Path.Combine(basePath, UploadsFolder);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string content = string.Empty;

            if (Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                content = ExtractTextFromPdf(filePath);
            }
            else if (Path.GetExtension(filePath).Equals(".doc", StringComparison.OrdinalIgnoreCase) ||
                     Path.GetExtension(filePath).Equals(".docx", StringComparison.OrdinalIgnoreCase))
            {
                content = ExtractTextFromWord(filePath);
            }
            else
            {
                return BadRequest("Unsupported file format.");
            }

            var resume = new Resume
            {
                FileName = file.FileName,
                FilePath = filePath,
                Content = content
            };

            _context.Resumes.Add(resume);
            await _context.SaveChangesAsync();

            return Ok("File uploaded and processed successfully.");
        }

        private string ExtractTextFromPdf(string filePath)
        {
            using (var pdf = PdfDocument.Open(filePath))
            {
                var text = new StringBuilder();
                foreach (var page in pdf.GetPages())
                {
                    text.Append(page.Text);
                }
                return text.ToString();
            }
        }

        private string ExtractTextFromWord(string filePath)
        {
            var document = new Aspose.Words.Document(filePath);
            return document.ToString(SaveFormat.Text);
        }



        
        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {

            if(string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("No search criteria provided");
            }

            //comma search
            var delimiters = new[] { ',', '/', ' ' };


            var searchTerms = query.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
                .Select(term => term.Trim())
                .ToArray();

            //var resumes = await _context.Resumes
            //    .Where(BuildSearchExpression(searchTerms))
            //    .ToListAsync();

            var resumes = await _context.Resumes
                .Where(BuildSearchExpression(searchTerms))
                .Select(r => new
                {
                    Id = r.Id,
                    FileName = r.FileName,
                    PdfUrl = Url.Action(nameof(DownloadResume), "Resumes", new { id = r.Id }, Request.Scheme)
                }).ToListAsync();

            if (!resumes.Any())
                return NotFound("No MAching Resume found");
            return Ok(resumes);

        }
        private Expression<Func<Resume, bool>> BuildSearchExpression(string[] searchTerms)
        {
            if (searchTerms.Length == 0 || searchTerms == null)
            {
                return r => false;
            }

            //check resume and it`s content using LINQ Expression-parameter object and property-content
            var parameter = Expression.Parameter(typeof(Resume), "r");
            var property = Expression.Property(parameter, nameof(Resume.Content) );

            //ensure property is string type
            if(property.Type != typeof(string))
            {
                throw new InvalidOperationException($"Property {nameof(Resume.Content)} is not supported.");
            }

            //combines all the search expression
            Expression combined = null;

            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            //check if contains method exist
            
            if(containsMethod == null)
            {
                throw new InvalidOperationException("Unable to find Contains method");
            }

            foreach (var term in searchTerms)
            {
                var searchTerm = Expression.Constant(term);
                //var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(property, containsMethod, searchTerm);

                if (combined == null)
                {
                    combined = containsExpression;
                }
                else
                {
                    //for Or logib

                    //combined = Expression.OrElse(combined, containsExpression);

                    //for AND logic
                    combined = Expression.AndAlso(combined, containsExpression);
                }
            }
            return Expression.Lambda<Func<Resume, bool>>(combined, parameter);
        }

        
        
    }

    
}
