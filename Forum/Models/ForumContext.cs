using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forum.Models;

public class ForumContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Thema> Themas { get; set; }
    public DbSet<Message> Messages { get; set; }
    
    public ForumContext(DbContextOptions<ForumContext> options) : base(options) {}
}