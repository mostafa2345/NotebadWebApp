using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Notebad.Data;
using Notebad.Models;

namespace Notebad.Controllers
{
    [Authorize]
    public class DocsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;

        public DocsController(ApplicationDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: Docs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext =from c in _context.Docs select c;
            applicationDbContext = applicationDbContext.Where(d => d.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(await applicationDbContext.Include(d => d.User).ToListAsync());
        }

  
       
        // GET: Docs/Create
        public IActionResult Create()
        {
           
            return View();
        }

        // POST: Docs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,UserId")] Doc doc)
        {
            if (ModelState.IsValid)
            {
                // Valid input: Save to the database, etc.
                _context.Add(doc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Invalid input: Handle validation errors
            // You can access specific errors like this:
            var titleErrors = ModelState["Title"].Errors;
            var contentErrors = ModelState["Content"].Errors;
            

            // Return the view with error messages
            return View(doc);
        }

        // GET: Docs/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doc = await _context.Docs.FindAsync(id);
            if (doc == null)
            {
                return NotFound();
            }  
            if (doc.UserId!=User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }
 
            return View(doc);
        }

        // POST: Docs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,UserId")] Doc doc)
        {
            if (id != doc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocExists(doc.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", doc.UserId);
            return View(doc);
        }

        // GET: Docs/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doc = await _context.Docs
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doc == null)
            {
                return NotFound();
            }
            if (doc.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View(doc);
        }

        // POST: Docs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doc = await _context.Docs.FindAsync(id);
            if (doc != null)
            {
                _context.Docs.Remove(doc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocExists(int id)
        {
            return _context.Docs.Any(e => e.Id == id);
        }
    }
}
