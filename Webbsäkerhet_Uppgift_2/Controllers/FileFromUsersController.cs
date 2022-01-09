using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using Webbsäkerhet_Uppgift_2.Models;
using Webbsäkerhet_Uppgift_2.Utilities;

namespace Webbsäkerhet_Uppgift_2.Controllers
{
    public class FileFromUsersController : Controller
    {
        private readonly WebSafetyContext _context;
        private readonly long fileSizeLimit = 10 * 1048576;
        private readonly string[] allowedExtensions = { ".png", ".jpg" };

        public FileFromUsersController(WebSafetyContext context)
        {
            _context = context;
        }

        // GET: FileFromUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.FileFromUser.ToListAsync());
        }



        // GET: FileFromUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FileFromUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TimeStamp,UntrustedName,FileSize,Content")] FileFromUser fileFromUser)
        {
            if (ModelState.IsValid)
            {
                fileFromUser.Id = Guid.NewGuid();
                _context.Add(fileFromUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fileFromUser);
        }

        // GET: FileFromUsers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileFromUser = await _context.FileFromUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileFromUser == null)
            {
                return NotFound();
            }

            return View(fileFromUser);
        }

        // POST: FileFromUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fileFromUser = await _context.FileFromUser.FindAsync(id);
            _context.FileFromUser.Remove(fileFromUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileFromUserExists(Guid id)
        {
            return _context.FileFromUser.Any(e => e.Id == id);
        }

        public async Task<IActionResult> UploadFile()
        {
            var webrequest = HttpContext.Request;

            if (!webrequest.HasFormContentType || !MediaTypeHeaderValue.TryParse(webrequest.ContentType, out var mediaTypeHeaderValue) || string.IsNullOrEmpty(mediaTypeHeaderValue.Boundary.Value))
            {

                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeaderValue.Boundary.Value, webrequest.Body);
            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                var checkContentDispostionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDispositionHeaderValue);
                if (checkContentDispostionHeader && contentDispositionHeaderValue.DispositionType.Equals("form-data") && !string.IsNullOrEmpty(contentDispositionHeaderValue.FileName.Value))
                {
                    FileFromUser fileFromUser = new FileFromUser();
                    fileFromUser.UntrustedName = HttpUtility.HtmlEncode(contentDispositionHeaderValue.FileName.Value);
                    fileFromUser.TimeStamp = DateTime.Now;
                    fileFromUser.Content = await FileHelper.ProcessStreamedFile(section, contentDispositionHeaderValue, ModelState, allowedExtensions, fileSizeLimit);
                    if (fileFromUser.Content.Length == 0)
                    {
                        return RedirectToAction("Index", "FileFromUsers");
                    }
                    fileFromUser.FileSize = fileFromUser.Content.Length;
                    await _context.FileFromUser.AddAsync(fileFromUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "FileFromUsers");
                }
                section = await reader.ReadNextSectionAsync();
            }

        
            return BadRequest("No files data in the request.");
        }

        public async Task<IActionResult> Download(Guid id)
        {
            var file = await _context.FileFromUser.FirstOrDefaultAsync(f => f.Id == id);
            if(file != null)
            {
                return File(file.Content, MediaTypeNames.Application.Octet,file.UntrustedName );
            }
            else
            {
                return NotFound();
            }
        }
    }
}