using Forum.Models;
using Microsoft.AspNetCore.Identity;

namespace Forum.Repositories;

public class UserRepository : IUserRepository
{
    public ForumContext _context { get; set; }
    public UserManager<User> _userManager { get; set; }

    public UserRepository(ForumContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User?> FindByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }
}