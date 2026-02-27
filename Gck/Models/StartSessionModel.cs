namespace Gck.Models;

public class StartSessionModel
{
    public int SeatsCount { get; set; } = 1;
    public int AnonymousCustomersCount { get; set; } = 0;
    public List<int> CustomerIds { get; set; } = new();
}
