using Microsoft.AspNetCore.Components;

namespace Gck.Components;

public partial class SeoHead : ComponentBase
{
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? Description { get; set; }
    [Parameter] public string? Keywords { get; set; }
    [Parameter] public string? Image { get; set; }
    [Parameter] public string? Url { get; set; }
    [Parameter] public string? Type { get; set; } = "website";
    [Parameter] public bool Index { get; set; } = true;
    [Parameter] public bool Follow { get; set; } = true;

    private string GetRobotsContent()
    {
        var robots = new List<string>();
        
        if (Index) robots.Add("index");
        else robots.Add("noindex");
        
        if (Follow) robots.Add("follow");
        else robots.Add("nofollow");
        
        return string.Join(", ", robots);
    }

    private string GetCanonicalUrl()
    {
        return Url ?? "https://gckgames.ir";
    }

    private string GetImageUrl()
    {
        return Image ?? "https://gckgames.ir/images/og-image.jpg";
    }
}