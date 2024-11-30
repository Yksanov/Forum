using Forum.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ForumContext>(options => options.UseNpgsql(connection))
    .AddIdentity<User, IdentityRole<int>>(options =>
    {
        // Настройки пароля
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        // Настройки пользователя
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ForumContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Thema}/{action=Index}/{id?}");

app.Run();