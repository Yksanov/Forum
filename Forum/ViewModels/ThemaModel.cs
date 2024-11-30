using Forum.Models;

namespace Forum.ViewModels;

public class ThemaModel
{
    public PageViewModel PageViewModel { get; set; }
    public IEnumerable<Thema> Themas { get; set; }
}