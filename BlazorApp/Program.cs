using BlazorApp;
using BlazorApp.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ActivityLogService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    string connectionString = configuration.GetConnectionString("MongoDbConnection") ?? "mongodb://localhost:27017";
    string databaseName = configuration.GetSection("MongoDbSettings")?["DatabaseName"] ?? "mongo_activity_logs";
    return new ActivityLogService(connectionString, databaseName);
});

builder.Services.AddScoped<ActivityLogServiceFacade>();

builder.Services.AddSingleton<AppDbContextLogger>(sp =>
    new AppDbContextLogger(sp));

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var logInterceptor = sp.GetRequiredService<AppDbContextLogger>();
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(logInterceptor);
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LibraryFacade>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
