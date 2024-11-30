using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Forum.Models;
using Forum.ViewModels;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace Forum.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IWebHostEnvironment _environment;
    
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
    }
    
    [HttpGet]
    public IActionResult Login(string returnUrl = "")
    {
        return View(new LoginViewModel {ReturnUrl = returnUrl});
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.FindByEmailAsync(model.UserNameOrEmail);

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.UserNameOrEmail);
            }
            
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден");
                return View(model);
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            
             if (result.Succeeded)
             {
                 if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                 {
                     return Redirect(model.ReturnUrl);
                 }

                 return RedirectToAction("Index", "Thema");
             }
             
             ModelState.AddModelError(string.Empty, "Неправильные логин или пароль");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            string fileName = $"avatar_{model.Email}{Path.GetExtension(model.Avatar.FileName)}";
            
            if (model.Avatar != null && model.Avatar.Length > 0 && model.Avatar.ContentType.StartsWith("image/"))
            {
                string filePath = Path.Combine(_environment.WebRootPath, "avatars", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Avatar.CopyToAsync(stream);
                }
            }
            else
            {
                ModelState.AddModelError("Avatar", "Аватар может быть только картинкой");
                return View(model);
            }
            
            User user = new User()
            {
                UserName = model.UserName.ToLower(),
                Email = model.Email,
                PathToAvatarPhoto = $"/avatars/{fileName}"
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                // await _userManager.AddToRoleAsync(user, "user");
                return RedirectToAction("Index", "Thema");
            }

            foreach (IdentityError error in result.Errors) 
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Thema");
    }
}