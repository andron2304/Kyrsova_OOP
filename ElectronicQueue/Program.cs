using ElectronicQueue.Components;
using ElectronicQueue.Models; // ЗМІНЕНО: Підключаємо нову папку Models, де тепер лежить наша ООП-логіка

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ЗМІНЕНО: Реєструємо нашу нову систему черги (QueueSystem замість старого QueueService) 
// як Singleton (один спільний об'єкт для всіх сторінок сайту)
builder.Services.AddSingleton<QueueSystem>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();