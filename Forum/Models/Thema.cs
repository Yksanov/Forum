namespace Forum.Models;

public class Thema
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; } 
    public User? User { get; set; }

    public List<Message> Messages { get; set; } = new List<Message>();
}