using Asp.netCore_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
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
/** Configure IdentiyUser Setting With Password Options */
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
{
    option.Password.RequiredLength = 3;
    option.Password.RequireDigit = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireUppercase = false;
    option.Password.RequiredUniqueChars = 3;
    option.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>();

/** Change Access Denied Route Configuration */
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = new PathString("/Administration/AccessDenied");
});

builder.Services.AddControllersWithViews(config =>
{
    /** Cofigure Authorization Polic where user is authorize or not */
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

/** Configure Nlog Setting */
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

/** Add Claim Policy */
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));

    option.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role", "true"));

    option.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
});

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
/** add authentication middleware for check whether user is login or not */
app.UseAuthentication();
/** add authrization middleware to check whether current user is authorize or not */
app.UseAuthorization();

/** Configure MVC controller with default routes */
//app.UseMvcWithDefaultRoute();

/** Configure Conventional Routing With Default Routes */
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();