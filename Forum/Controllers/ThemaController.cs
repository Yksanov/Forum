using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Forum.Controllers
{
[Authorize]
    public class ThemaController : Controller
    {
        private readonly ForumContext _context;
        private readonly UserManager<User> _userManager;

        public ThemaController(ForumContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            User user = await _userManager.GetUserAsync(User);
            List<Message> messages = await _context.Messages.Where(m => m.UserId == user.Id).ToListAsync();

            int count = messages.Count;
            ViewBag.CountMessages = count;
            
            return View(viewModel);
        }
        
        public async Task<IActionResult> Details(int? id, int page = 1)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thema = _context.Themas.Include(t => t.User).FirstOrDefault(t => t.Id == id);

            IQueryable<Message> messagesQuery = _context.Messages.Where(m => m.ThemaId == thema.Id)
                .Include(r => r.User).ThenInclude(m => m.Messages);
            if (thema == null)
            {
                return NotFound();
            }

            int pageSize = 5;
            int count = await messagesQuery.CountAsync();
            
            List<Message> messages = await messagesQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            thema.Messages = messages;
            
            var currentUser = _userManager.GetUserAsync(User).Result;

            ChatViewModel viewModel = new ChatViewModel
            {
                Thema = thema,
                Messages = thema.Messages,
                CurrentUser = currentUser,
                PageViewModel = new PageViewModel(count, page, pageSize)
            };
            
            if (User.Identity.IsAuthenticated)
            {
                viewModel.CurrentUser = await _userManager.GetUserAsync(User);
            }

            return View(viewModel);
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
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                thema.UserId = user.Id;

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt, UserId")] Thema thema)
        {
            if (id != thema.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return Unauthorized();
                    }

                    thema.UserId = user.Id;

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
                return RedirectToAction("Details", "Thema", new{id = thema.Id});
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
        
        //---------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> SendMessage(string text, int topicId)
        {
            if (text == null)
            {
                return NotFound();
            }
            User user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            Message message = new Message
            {
                Text = text,
                User = user,
                ThemaId = topicId,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            List<Message> messages = await _context.Messages
                .Include(m => m.User)
                .Where(m => m.ThemaId == topicId) 
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            ChatViewModel model = new ChatViewModel()
            {
                CurrentUser = user,
                Messages = messages
            };

            //return PartialView("_MessagePartialView", model); 
            return PartialView("_SingleMessagePartialView", message);
        }


        public async Task<IActionResult> GetMessages(int topicId, int page = 1)
        {
            Thema? thema = await _context.Themas.FirstOrDefaultAsync(t => t.Id.ToString() == topicId.ToString());

            if (thema == null)
            {
                return NotFound();
            }
            
            IQueryable<Message> messagesQuery = _context.Messages.Where(r => r.ThemaId == thema.Id)
                .Include(r => r.User)
                .ThenInclude(u => u.Messages)
                .OrderBy(r => r.CreatedAt);
            
            if (thema == null)
            {
                return NotFound();
            }
            
            int pageSize = 5;
            int count = await messagesQuery.CountAsync();

            List<Message> messages = await messagesQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            
            User currentUser = await _userManager.GetUserAsync(User);
            // List<Message> messages = await _context.Messages.Include(m => m.User).Where(m => m.ThemaId == topicId)
            //     .OrderByDescending(m => m.CreatedAt).ToListAsync();
            
            ChatViewModel model = new ChatViewModel()
            {
                CurrentUser = currentUser,
                Messages = messages,
                PageViewModel = new PageViewModel(count, page, pageSize)
            };
            return PartialView("_MessagePartialView", model);
        }
    }
}