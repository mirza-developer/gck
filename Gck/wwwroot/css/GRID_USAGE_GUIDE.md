# Data Grid System - Usage Guide

This guide explains how to use the reusable data grid system with expandable row details across your Blazor application.

## Features

- ? Clean, modern table-based grid layout
- ? Expandable row details with smooth animations
- ? Responsive design (mobile, tablet, desktop)
- ? RTL (Right-to-Left) support for Persian/Arabic
- ? Gaming-themed styling with purple/pink gradients
- ? Customizable cell types (primary, secondary, badges)
- ? Built-in action buttons (edit, delete, custom)
- ? Empty state handling
- ? Integrated with PersianResources for localization

## Basic Structure

```razor
@using Gck.Resources

<div class="data-grid">
    <table class="data-grid-table">
        <thead class="data-grid-header">
            <tr>
                <th></th> <!-- For expand toggle button -->
                <th>@PersianResources.CustomerName</th>
                <th>@PersianResources.Phone</th>
                <th>@PersianResources.Actions</th>
            </tr>
        </thead>
        <tbody class="data-grid-body">
            @foreach (var item in items)
            {
                <tr class="@(isExpanded ? "expanded" : "")">
                    <td>
                        <button class="grid-expand-toggle @(isExpanded ? "expanded" : "")" 
                                @onclick="() => ToggleRow(item.Id)">
                            <i class="fas fa-chevron-down"></i>
                        </button>
                    </td>
                    <td><span class="grid-cell-primary">@item.Name</span></td>
                    <td><span class="grid-cell-secondary">@item.Detail</span></td>
                    <td>
                        <div class="grid-actions">
                            <button class="grid-action-btn edit" @onclick="() => Edit(item.Id)">
                                <i class="fas fa-edit"></i>
                                @PersianResources.Edit
                            </button>
                            <button class="grid-action-btn delete" @onclick="() => Delete(item.Id)">
                                <i class="fas fa-trash"></i>
                                @PersianResources.Delete
                            </button>
                        </div>
                    </td>
                </tr>
                @if (isExpanded)
                {
                    <tr class="grid-details-row">
                        <td colspan="4" class="grid-details-cell">
                            <div class="grid-details-content">
                                <!-- Details content here -->
                            </div>
                        </td>
                    </tr>
                }
            }
        </tbody>
    </table>
</div>
```

## Using PersianResources

Always use `PersianResources` constants instead of hardcoded Persian text for consistency and maintainability:

```razor
@using Gck.Resources

<!-- ? GOOD -->
<h4>@PersianResources.CustomerFullDetails</h4>
<button>@PersianResources.Edit</button>

<!-- ? BAD -->
<h4>?????? ???? ?????</h4>
<button>??????</button>
```

### Available Resource Strings for Grid

| Constant | Value |
|----------|-------|
| `PersianResources.Actions` | ?????? |
| `PersianResources.Details` | ?????? |
| `PersianResources.FullDetails` | ?????? ???? |
| `PersianResources.Id` | ????? |
| `PersianResources.Edit` | ?????? |
| `PersianResources.Delete` | ??? |
| `PersianResources.CustomerName` | ??? ????? |
| `PersianResources.Phone` | ???? |
| `PersianResources.BirthYear` | ??? ???? |
| `PersianResources.Gender` | ????? |
| `PersianResources.Male` | ??? |
| `PersianResources.Female` | ?? |
| `PersianResources.ApproximateAge` | ?? ?????? |
| `PersianResources.Year` | ??? |
| `PersianResources.Shamsi` | ???? |

## CSS Classes Reference

### Main Grid Container

| Class | Purpose |
|-------|---------|
| `.data-grid` | Main container for the entire grid |
| `.data-grid-table` | Table element with proper styling |
| `.data-grid-header` | Table header with gradient background |
| `.data-grid-body` | Table body with row hover effects |

### Cell Styling

| Class | Purpose | Example |
|-------|---------|---------|
| `.grid-cell-primary` | Primary data (names, titles) | `<span class="grid-cell-primary">@customer.Name</span>` |
| `.grid-cell-secondary` | Secondary data (IDs, dates) | `<span class="grid-cell-secondary">@customer.BirthYear</span>` |
| `.grid-cell-badge` | Badge-style cells | See badge section below |

### Badges

```razor
<!-- Male badge -->
<span class="grid-cell-badge male">
    <i class="fas fa-mars"></i>
    @PersianResources.Male
</span>

<!-- Female badge -->
<span class="grid-cell-badge female">
    <i class="fas fa-venus"></i>
    @PersianResources.Female
</span>
```

**Note:** You can create custom badge styles by extending the `.grid-cell-badge` class:

```css
.grid-cell-badge.active {
    background: rgba(0, 255, 0, 0.2);
    color: #00ff00;
    border: 1px solid rgba(0, 255, 0, 0.3);
}
```

### Expandable Row Details

| Class | Purpose |
|-------|---------|
| `.grid-expand-toggle` | Button to expand/collapse row |
| `.grid-expand-toggle.expanded` | Expanded state (rotates icon) |
| `.grid-details-row` | Container row for details |
| `.grid-details-cell` | Cell spanning all columns |
| `.grid-details-content` | Content wrapper with padding |
| `.grid-details-header` | Header section in details |
| `.grid-details-body` | Body section with responsive grid |

### Detail Items

```razor
<div class="grid-details-body">
    <div class="grid-detail-item">
        <div class="grid-detail-label">
            <i class="fas fa-user"></i>
            @PersianResources.CustomerName
        </div>
        <div class="grid-detail-value">
            @customer.Name
        </div>
    </div>
</div>
```

| Class | Purpose |
|-------|---------|
| `.grid-detail-item` | Individual detail field container |
| `.grid-detail-label` | Label with icon |
| `.grid-detail-value` | Value text |
| `.grid-detail-value.highlight` | Highlighted value (cyan color) |

### Action Buttons

```razor
<div class="grid-actions">
    <button class="grid-action-btn edit">
        <i class="fas fa-edit"></i>
        @PersianResources.Edit
    </button>
    <button class="grid-action-btn delete">
        <i class="fas fa-trash"></i>
        @PersianResources.Delete
    </button>
</div>
```

| Class | Style |
|-------|-------|
| `.grid-action-btn` | Base button style |
| `.grid-action-btn.edit` | Cyan gradient (edit action) |
| `.grid-action-btn.delete` | Pink gradient (delete action) |

## Complete Example: Customers Grid

```razor
@page "/customers"
@using Gck.Application.DTOs
@using Gck.Resources

@code {
    private int? expandedRowId = null;

    private void ToggleRow(int customerId)
    {
        expandedRowId = expandedRowId == customerId ? null : customerId;
    }
}

<div class="data-grid">
    <table class="data-grid-table">
        <thead class="data-grid-header">
            <tr>
                <th></th>
                <th>@PersianResources.CustomerName</th>
                <th>@PersianResources.Phone</th>
                <th>@PersianResources.BirthYear</th>
                <th>@PersianResources.Gender</th>
                <th>@PersianResources.Actions</th>
            </tr>
        </thead>
        <tbody class="data-grid-body">
            @foreach (var customer in customers)
            {
                var isExpanded = expandedRowId == customer.Id;
                <tr class="@(isExpanded ? "expanded" : "")">
                    <td>
                        <button class="grid-expand-toggle @(isExpanded ? "expanded" : "")" 
                                @onclick="() => ToggleRow(customer.Id)">
                            <i class="fas fa-chevron-down"></i>
                        </button>
                    </td>
                    <td>
                        <span class="grid-cell-primary">@customer.Name</span>
                    </td>
                    <td>
                        <span class="grid-cell-secondary">@customer.PhoneNumber</span>
                    </td>
                    <td>
                        <span class="grid-cell-secondary">@customer.BirthYear</span>
                    </td>
                    <td>
                        <span class="grid-cell-badge @(customer.Gender == "Male" ? "male" : "female")">
                            <i class="fas @(customer.Gender == "Male" ? "fa-mars" : "fa-venus")"></i>
                            @(customer.Gender == "Male" ? PersianResources.Male : PersianResources.Female)
                        </span>
                    </td>
                    <td>
                        <div class="grid-actions">
                            <button class="grid-action-btn edit" @onclick="() => EditCustomer(customer.Id)">
                                <i class="fas fa-edit"></i>
                                @PersianResources.Edit
                            </button>
                            <button class="grid-action-btn delete" @onclick="() => DeleteCustomer(customer.Id)">
                                <i class="fas fa-trash"></i>
                                @PersianResources.Delete
                            </button>
                        </div>
                    </td>
                </tr>
                @if (isExpanded)
                {
                    <tr class="grid-details-row">
                        <td colspan="6" class="grid-details-cell">
                            <div class="grid-details-content">
                                <div class="grid-details-header">
                                    <i class="fas fa-info-circle"></i>
                                    <h4>@PersianResources.CustomerFullDetails</h4>
                                </div>
                                <div class="grid-details-body">
                                    <div class="grid-detail-item">
                                        <div class="grid-detail-label">
                                            <i class="fas fa-id-card"></i>
                                            @PersianResources.Id
                                        </div>
                                        <div class="grid-detail-value highlight">
                                            #@customer.Id
                                        </div>
                                    </div>
                                    <div class="grid-detail-item">
                                        <div class="grid-detail-label">
                                            <i class="fas fa-user"></i>
                                            @PersianResources.CustomerName
                                        </div>
                                        <div class="grid-detail-value">
                                            @customer.Name
                                        </div>
                                    </div>
                                    <div class="grid-detail-item">
                                        <div class="grid-detail-label">
                                            <i class="fas fa-phone"></i>
                                            @PersianResources.Phone
                                        </div>
                                        <div class="grid-detail-value">
                                            @customer.PhoneNumber
                                        </div>
                                    </div>
                                    <div class="grid-detail-item">
                                        <div class="grid-detail-label">
                                            <i class="fas fa-calendar-alt"></i>
                                            @PersianResources.BirthYear
                                        </div>
                                        <div class="grid-detail-value">
                                            @customer.BirthYear @PersianResources.Shamsi
                                        </div>
                                    </div>
                                    <div class="grid-detail-item">
                                        <div class="grid-detail-label">
                                            <i class="fas @(customer.Gender == "Male" ? "fa-mars" : "fa-venus")"></i>
                                            @PersianResources.Gender
                                        </div>
                                        <div class="grid-detail-value">
                                            @(customer.Gender == "Male" ? PersianResources.Male : PersianResources.Female)
                                        </div>
                                    </div>
                                    <div class="grid-detail-item">
                                        <div class="grid-detail-label">
                                            <i class="fas fa-calculator"></i>
                                            @PersianResources.ApproximateAge
                                        </div>
                                        <div class="grid-detail-value highlight">
                                            @CalculateAge(customer.BirthYear) @PersianResources.Year
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                }
            }
        </tbody>
    </table>
</div>
```

## Responsive Behavior

The grid automatically adjusts for different screen sizes:

- **Desktop (>1024px)**: Full layout with all columns visible
- **Tablet (768px-1024px)**: Slightly reduced padding and font sizes
- **Mobile (<768px)**: 
  - Stacked action buttons
  - Single column detail layout
  - Reduced padding

## Customization Tips

### 1. Custom Badge Colors

```css
.grid-cell-badge.premium {
    background: rgba(255, 215, 0, 0.2);
    color: gold;
    border: 1px solid rgba(255, 215, 0, 0.3);
}
```

### 2. Additional Action Button Styles

```css
.grid-action-btn.view {
    background: linear-gradient(135deg, rgba(0, 255, 0, 0.2), rgba(0, 200, 0, 0.2));
    color: #00ff00;
    border: 1px solid rgba(0, 255, 0, 0.3);
}

.grid-action-btn.view:hover {
    background: linear-gradient(135deg, #00ff00, #00cc00);
    color: var(--darker-bg);
    box-shadow: 0 0 15px rgba(0, 255, 0, 0.5);
}
```

### 3. Different Detail Layouts

You can customize the grid layout in `.grid-details-body`:

```razor
<!-- Two columns on desktop, one on mobile -->
<div class="grid-details-body" style="grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));">
    <!-- items -->
</div>

<!-- Fixed 3-column layout -->
<div class="grid-details-body" style="grid-template-columns: repeat(3, 1fr);">
    <!-- items -->
</div>
```

## Empty State

When there's no data, use PersianResources:

```razor
@if (!items.Any())
{
    <div class="grid-empty-state">
        <i class="fas fa-inbox"></i>
        <p>@PersianResources.NoCustomersFound</p>
    </div>
}
```

## Adding New Resource Strings

When you need new text for the grid, add it to `Gck\Resources\PersianResources.cs`:

```csharp
public static class PersianResources
{
    // ...existing code...
    
    // Add your new constants here
    public const string MyNewLabel = "????? ???? ??";
}
```

Then use it in your Razor page:

```razor
@using Gck.Resources

<span>@PersianResources.MyNewLabel</span>
```

## Accessibility

- All buttons have proper hover states
- Color contrast meets WCAG standards
- Keyboard navigation supported
- Screen reader friendly with semantic HTML

## Performance Tips

1. **Limit expanded rows**: Only expand one row at a time (as shown in examples)
2. **Lazy load details**: Load detail data only when row is expanded
3. **Virtual scrolling**: For very large datasets, consider virtualization
4. **Debounce search**: When implementing search filters

## See It In Action

Check out the implementation in:
- `Gck\Pages\Customers\Index.razor` - Full example with all features and PersianResources integration

## Browser Support

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)
