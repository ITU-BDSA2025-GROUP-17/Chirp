using Microsoft.EntityFrameworkCore;
using Chirp.Repositories;
using Chirp.Core;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
var builder = WebApplication.CreateBuilder(args);

// Load database connection via configuration


if (builder.Environment.IsEnvironment("Testing"))
{
    var connection = new SqliteConnection("Data Source=:memory:");
    connection.Open();

    builder.Services.AddSingleton(connection);

    builder.Services.AddDbContext<CheepDBContext>((sp, options) =>
    {
        var conn = sp.GetRequiredService<SqliteConnection>();
        options.UseSqlite(conn);
    });
} else
{
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<CheepDBContext>(options => options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Web")));
}
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity type for site
builder.Services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CheepDBContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

// CORS polic y
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()!;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowChirp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // for OAuth
    });
});

// Conf SameSite cookie behavior (protect against CSRF)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// For Github OAuth
builder.Services.AddSession();

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddAuthentication().AddCookie();
} else
{
    builder.Services.AddAuthentication()
    .AddGitHub(o =>
    {
        o.ClientId = builder.Configuration["authentication:github:clientId"]!;
        o.ClientSecret = builder.Configuration["authentication:github:clientSecret"]!;
        o.CallbackPath = "/signin-github";
        o.Scope.Add("user:email");
        o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        o.ClaimActions.MapJsonKey("urn:github:login", "login");

    });
}

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<CheepDBContext>();

    // Optional: for Identity users if you want to seed roles/users
    var userManager = services.GetRequiredService<UserManager<Author>>();

    if (builder.Environment.IsEnvironment("Testing"))
        dbContext.Database.EnsureCreated();      
    else
        dbContext.Database.Migrate();

    DbInitializer.SeedDatabase(dbContext);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//middleware
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowChirp");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
