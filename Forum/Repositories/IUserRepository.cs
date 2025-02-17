using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Repositories;

public interface IUserRepository
{
    ForumContext _context { get; set; }

    async Task<List<User>> GetAllUserAsync() => await _context.Users.ToListAsync();
    async Task<User?> GetUserByIdAsync(int id) => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> FindByEmailAsync(string email);
    
    public Task<User?> FindByNameAsync(string userName);
}