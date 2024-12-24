using Forum.Models;

namespace Forum.Repositories;

public class UserRepository : IUserRepository
{
    public ForumContext _context { get; set; }

    public UserRepository(ForumContext context)
    {
        _context = context;
    }
}