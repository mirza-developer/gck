using Microsoft.AspNetCore.Components;

namespace Gck.Components;

public partial class LoadingSpinner
{
    [Parameter]
    public string LoadingText { get; set; } = "در حال بارگیری...";

    [Parameter]
    public string ContainerClass { get; set; } = string.Empty;

    [Parameter]
    public string ContainerStyle { get; set; } = string.Empty;

    [Parameter]
    public bool Fullscreen { get; set; } = true;

    protected override void OnParametersSet()
    {
        if (!Fullscreen && string.IsNullOrWhiteSpace(ContainerStyle))
        {
            ContainerStyle = "height: auto; min-height: 200px;";
        }
    }

    private string GetContainerClasses()
    {
        var classes = new List<string>();

        if (!Fullscreen)
        {
            classes.Add("inline");
        }

        if (!string.IsNullOrWhiteSpace(ContainerClass))
        {
            classes.Add(ContainerClass);
        }

        return string.Join(" ", classes);
    }
}
