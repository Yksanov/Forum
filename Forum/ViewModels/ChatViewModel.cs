using Forum.Models;

namespace Forum.ViewModels;

public class ChatViewModel
{
    public Thema Thema { get; set; }
    public User CurrentUser { get; set; }
    public List<Message> Messages { get; set; }
    public PageViewModel PageViewModel { get; set; }
}