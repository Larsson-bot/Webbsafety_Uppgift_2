using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Webbsäkerhet_Uppgift_2.Models;

namespace Webbsäkerhet_Uppgift_2.Controllers
{
    public class CommentsController : Controller
    {
        private readonly WebSafetyContext db;
        private List<string> allowedTag { get; set; }

        public CommentsController(WebSafetyContext context)
        {
            db = context;
            allowedTag = new List<string>() { "<b>", "</b>", "<u>", "</u>" };
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            return View(await db.Comment.ToListAsync());
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Id = Guid.NewGuid();
                comment.TimeStamp = DateTime.Now;
                if (!string.IsNullOrEmpty(comment.Content))
                {
                    string encodedString = HttpUtility.HtmlEncode(comment.Content);
                    foreach (var tag in allowedTag)
                    {
                        string encodedTag = HttpUtility.HtmlEncode(tag);
                        encodedString = encodedString.Replace(encodedTag, tag);
                    }
                    comment.Content = encodedString;
                    db.Add(comment);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("ErrorDisplay", "Error: Please write something.");
                    return View(comment);
                }
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await db.Comment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var comment = await db.Comment.FindAsync(id);
            db.Comment.Remove(comment);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}