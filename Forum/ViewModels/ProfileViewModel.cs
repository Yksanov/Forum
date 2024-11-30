using Forum.Models;

namespace Forum.ViewModels;

public class ProfileViewModel
{
    public User User { get; set; }
    
    public User CurrentUser { get; set; }
}