using Asp.netCore_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

/** 
 * Add services to the container.
 */
/** Add MVC service */
//builder.Services.AddMvc(options =>
//{
//    /** check end point routing */
//    options.EnableEndpointRouting = false;
//});
builder.Services.AddDbContextPool<AppDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("EmployeeDbConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddControllersWithViews();

/** Configure Nlog Setting */
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();


/**
 * Configure the HTTP request pipeline.
 */
var app = builder.Build();

/** Check Development mode and add Developer Exception */
if (app.Environment.IsDevelopment())
{
    DeveloperExceptionPageOptions options = new DeveloperExceptionPageOptions();
    options.SourceCodeLineCount = 10;
    app.UseDeveloperExceptionPage(options);
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}"); // Default Status Code Error Page
}

/** add static file middleware for connect under wwwroot */
app.UseStaticFiles();
app.UseAuthentication();

/** Configure MVC controller with default routes */
//app.UseMvcWithDefaultRoute();

/** Configure Conventional Routing With Default Routes */
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();