using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Forum.Controllers
{
[Authorize]
    public class ThemaController : Controller
    {
        private readonly ForumContext _context;

        public ThemaController(ForumContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(int page = 1)
        {
            var forumContext = _context.Themas.Include(t => t.User);
            
            
            int pageSize = 5; 
            int totalItems = await forumContext.CountAsync(); 
            var items = await forumContext
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            
            ThemaModel viewModel = new ThemaModel
            {
                PageViewModel = new PageViewModel(totalItems, page, pageSize),
                Themas = items 
            };
            
            return View(viewModel);
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thema = await _context.Themas
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thema == null)
            {
                return NotFound();
            }

            return View(thema);
        }
        
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CreatedAt")] Thema thema)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thema);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(thema);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thema = await _context.Themas.FindAsync(id);
            if (thema == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", thema.UserId);
            return View(thema);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt")] Thema thema)
        {
            if (id != thema.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thema);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThemaExists(thema.Id))
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
            return View(thema);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thema = await _context.Themas
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thema == null)
            {
                return NotFound();
            }

            return View(thema);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thema = await _context.Themas.FindAsync(id);
            if (thema != null)
            {
                _context.Themas.Remove(thema);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThemaExists(int id)
        {
            return _context.Themas.Any(e => e.Id == id);
        }
    }
}
