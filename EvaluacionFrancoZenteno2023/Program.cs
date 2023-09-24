using EvaluacionFrancoZenteno2023.Helpers;
using EvaluacionFrancoZenteno2023.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AdventureWorksLt2019Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StringAdvertureWorks"))
);

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "UserLoginCookie";
        options.LoginPath = "/Login";
        //options.AccessDeniedPath = new PathString("/Home/AccessDenied");
    }
);

using var serviceScope = builder.Services.BuildServiceProvider().CreateScope();
var dbContext = serviceScope.ServiceProvider.GetRequiredService<AdventureWorksLt2019Context>();

//var permissionsList = dbContext.Permisos.Select(p => p.Descripcion).ToList();

//builder.Services.AddAuthorization(options =>
//{
//    // Add the DefaultGlobalAdmin policy
//    options.AddPolicy("DefaultGlobalAdmin", policy =>
//        policy.RequireAssertion(context =>
//            context.User.HasClaim(c =>
//                (c.Type == ClaimTypes.Role && c.Value == "Global Administrator"))));

//    foreach (var perm in permissionsList)
//    {
//        options.AddPolicy(perm, policy =>
//            policy.AddRequirements(new PermissionRequirement(perm)));
//    }
//});

//builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
