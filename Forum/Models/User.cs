using Microsoft.AspNetCore.Identity;

namespace Forum.Models;

public class User : IdentityUser<int>
{
    public string PathToAvatarPhoto { get; set; }
    
    public List<Thema> Themas { get; set; }
    public List<Message> Messages { get; set; }
}