using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers;

public class ValidationController : Controller
{
    private readonly UserManager<User> _userManager;

    public ValidationController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckUserEmail(string email)
    {
        User user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Json(true);
        }

        return Json(false);
    }
    
    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckUserName(string username)
    {
        User user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return Json(true);
        }

        return Json(false);
    }
}