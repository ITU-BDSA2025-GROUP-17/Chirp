using Microsoft.EntityFrameworkCore;
using Chirp.Repositories;
using Chirp.Core;

var builder = WebApplication.CreateBuilder(args);

// Load database connection via configuration
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CheepDBContext>(options => options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Web")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Identity type for site
builder.Services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CheepDBContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

// For Github OAuth
builder.Services.AddSession();

builder.Services.AddAuthentication()
    .AddGitHub(o =>
    {
        o.ClientId = builder.Configuration["authentication:github:clientId"];
        o.ClientSecret = builder.Configuration["authentication:github:clientSecret"];
        o.CallbackPath = "/signin-github";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
