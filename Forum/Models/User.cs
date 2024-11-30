using Microsoft.AspNetCore.Identity;

namespace Forum.Models;

public class User : IdentityUser<int>
{
    public DateOnly DateOfBirth { get; set; }
    public string PathToAvatarPhoto { get; set; }
}