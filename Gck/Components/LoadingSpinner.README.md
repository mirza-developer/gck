# LoadingSpinner Component

کامپوننت لودینگ اسپینر برای نمایش وضعیت بارگیری در برنامه Blazor با طراحی گیمینگ مدرن.

## ویژگی‌ها

- ✨ انیمیشن‌های روان و جذاب با سه حلقه چرخان
- 🎨 سازگار با تم Gen-Z Gaming با گرادیانت‌های رنگی
- 📱 طراحی Responsive
- ⚙️ قابلیت سفارشی‌سازی بالا
- 🌐 پشتیبانی کامل از RTL و زبان فارسی

## نحوه استفاده

### نصب

کامپوننت به صورت پیش‌فرض در پوشه `Components` قرار دارد و نیازی به نصب اضافی ندارد.

### استفاده ساده

```razor
@using Gck.Components

<LoadingSpinner />
```

### حالت‌های مختلف

#### 1. لودینگ تمام صفحه (پیش‌فرض)

```razor
<LoadingSpinner LoadingText="در حال بارگیری..." />
```

#### 2. لودینگ Inline

```razor
<LoadingSpinner 
    Fullscreen="false" 
    LoadingText="در حال بارگیری داده‌ها..." />
```

#### 3. لودینگ کوچک

```razor
<LoadingSpinner 
    Fullscreen="false" 
    LoadingText="لطفا صبر کنید..." 
    ContainerClass="small" />
```

#### 4. بدون متن

```razor
<LoadingSpinner 
    Fullscreen="false" 
    LoadingText="" />
```

#### 5. با استایل سفارشی

```razor
<LoadingSpinner 
    Fullscreen="false" 
    ContainerStyle="height: 300px; background: rgba(0,0,0,0.5);"
    LoadingText="بارگیری..." />
```

## پارامترها

| پارامتر | نوع | پیش‌فرض | توضیحات |
|---------|-----|---------|---------|
| `LoadingText` | `string` | `"در حال بارگیری..."` | متن نمایشی در حین بارگیری |
| `ContainerClass` | `string` | `string.Empty` | کلاس CSS اضافی برای کانتینر |
| `ContainerStyle` | `string` | `string.Empty` | استایل inline برای کانتینر |
| `Fullscreen` | `bool` | `true` | اگر true باشد، لودینگ تمام صفحه را می‌پوشاند |

## مثال‌های کاربردی

### نمایش در حین بارگیری داده

```razor
@page "/users"

@if (isLoading)
{
    <LoadingSpinner 
        Fullscreen="false" 
        LoadingText="در حال بارگیری کاربران..." />
}
else
{
    <div class="users-list">
        @foreach (var user in users)
        {
            <div>@user.Name</div>
        }
    </div>
}

@code {
    private bool isLoading = true;
    private List<User> users = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadUsersAsync();
        isLoading = false;
    }
}
```

### لودینگ با دکمه

```razor
<button @onclick="LoadDataAsync">بارگیری داده‌ها</button>

@if (isLoading)
{
    <LoadingSpinner 
        Fullscreen="false" 
        ContainerClass="small"
        LoadingText="در حال پردازش..." />
}

@code {
    private bool isLoading = false;

    private async Task LoadDataAsync()
    {
        isLoading = true;
        StateHasChanged();
        
        await Task.Delay(2000); // شبیه‌سازی عملیات
        
        isLoading = false;
    }
}
```

### لودینگ Modal

```razor
@if (showModal)
{
    <div class="modal-overlay">
        <div class="modal-content">
            <LoadingSpinner 
                Fullscreen="false"
                LoadingText="در حال ذخیره اطلاعات..." />
        </div>
    </div>
}
```

## سفارشی‌سازی استایل

برای سفارشی‌سازی بیشتر، می‌توانید از CSS Variables استفاده کنید:

```css
:root {
    --primary-purple: #6c5ce7;
    --secondary-pink: #fd79a8;
    --accent-cyan: #00cec9;
    --darker-bg: #0a0a1a;
}
```

یا مستقیماً استایل‌های کامپوننت را در `LoadingSpinner.razor.css` ویرایش کنید.

## دمو

برای مشاهده نمونه‌های مختلف استفاده از کامپوننت، به صفحه `/loading-demo` مراجعه کنید.

## ملاحظات

- اگر از حالت `Fullscreen="true"` استفاده می‌کنید، کامپوننت تمام صفحه را می‌پوشاند
- برای استفاده Inline، حتماً `Fullscreen="false"` را تنظیم کنید
- رنگ‌ها و انیمیشن‌ها از CSS Variables تم Gaming استفاده می‌کنند

## مثال استفاده در index.html

این کامپوننت بر اساس لودینگ HTML در `index.html` طراحی شده است:

```html
<div id="app">
    <div class="loading-container">
        <div class="gaming-loader">
            <div class="loader-ring"></div>
            <div class="loader-ring"></div>
            <div class="loader-ring"></div>
            <div class="loading-text">در حال بارگیری...</div>
        </div>
    </div>
</div>
```

اکنون می‌توانید همان ظاهر را در هر جای برنامه Blazor خود استفاده کنید!
