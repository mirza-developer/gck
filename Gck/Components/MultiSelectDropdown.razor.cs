using Gck.Resources;
using Microsoft.AspNetCore.Components;

namespace Gck.Components;

public partial class MultiSelectDropdown<TItem, TValue>
{
    [Parameter] public List<TItem> Items { get; set; } = new();
    [Parameter] public bool IsMultiSelect { get; set; } = true;
    [Parameter] public bool EnableSearch { get; set; } = false;
    [Parameter] public string Placeholder { get; set; } = PersianResources.SelectItem;
    [Parameter] public string SearchPlaceholder { get; set; } = PersianResources.SearchPlaceholder;
    [Parameter] public string NoResultsText { get; set; } = PersianResources.NoResultsFound;
    
    [Parameter] public List<TValue> SelectedValues { get; set; } = new();
    [Parameter] public EventCallback<List<TValue>> SelectedValuesChanged { get; set; }
    
    [Parameter] public TValue? SelectedValue { get; set; }
    [Parameter] public EventCallback<TValue?> SelectedValueChanged { get; set; }
    
    [Parameter] public Func<TItem, TValue> ValueSelector { get; set; } = null!;
    [Parameter] public Func<TItem, string> DisplayTextSelector { get; set; } = null!;

    private bool isDropdownOpen;
    private string searchText = string.Empty;
    private ElementReference dropdownRef;

    private List<TItem> FilteredItems
    {
        get
        {
            if (!EnableSearch || string.IsNullOrWhiteSpace(searchText))
            {
                return Items;
            }

            var searchLower = searchText.ToLower();
            return Items.Where(item => 
                GetDisplayText(item).ToLower().Contains(searchLower)
            ).ToList();
        }
    }

    private void ToggleDropdown()
    {
        isDropdownOpen = !isDropdownOpen;
        if (!isDropdownOpen)
        {
            searchText = string.Empty;
        }
    }

    public void CloseDropdown()
    {
        isDropdownOpen = false;
        searchText = string.Empty;
    }

    private void ClearSearch()
    {
        searchText = string.Empty;
    }

    private TValue GetValue(TItem item)
    {
        return ValueSelector(item);
    }

    private string GetDisplayText(TItem item)
    {
        return DisplayTextSelector(item);
    }

    private async Task HandleItemClick(TValue value)
    {
        if (IsMultiSelect)
        {
            if (SelectedValues.Contains(value))
            {
                SelectedValues.Remove(value);
            }
            else
            {
                SelectedValues.Add(value);
            }
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }
        else
        {
            SelectedValue = value;
            await SelectedValueChanged.InvokeAsync(SelectedValue);
            isDropdownOpen = false;
            searchText = string.Empty;
        }
    }

    private async Task RemoveValue(TValue value)
    {
        if (IsMultiSelect)
        {
            SelectedValues.Remove(value);
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }
    }
}
