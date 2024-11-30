using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;

public class UserController : Controller
{
    private readonly ForumContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _environment;

    public UserController(ForumContext context, UserManager<User> userManager,IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }
    
    public async Task<IActionResult> Profile(int? id)
    {
        User user = await _userManager.GetUserAsync(User);
        User currentUser = await _userManager.GetUserAsync(User);
        
        if (id != null)
        {
            user = await _userManager.FindByIdAsync(id.ToString());
        }
        
        if (user == null)
        {
            return NotFound();
        }
        
        List<Message> messages = await _context.Messages.Where(m => m.UserId == user.Id).ToListAsync();
        int count = messages.Count;
        ViewBag.CountMessages = count;
        return View(new ProfileViewModel()
        {
            User = user,
            CurrentUser = currentUser
        });
    }
}