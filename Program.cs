using Asp.netCore_MVC.Models;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddControllersWithViews();

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
    app.UseStatusCodePages(); // Default Status Code Error Page
}

/** add static file middleware for connect under wwwroot */
app.UseStaticFiles();

/** Configure MVC controller with default routes */
//app.UseMvcWithDefaultRoute();

/** Configure Conventional Routing With Default Routes */
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();