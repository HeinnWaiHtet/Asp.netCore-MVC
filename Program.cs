using Asp.netCore_MVC.Models;
using Asp.netCore_MVC.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

/** 
 * Add services to the container.
 */
/** Add Database Configuration */
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

    /** configure to comfirm Email when login */
    option.SignIn.RequireConfirmedEmail = true;

    /** Custom Email Lifespan Token */
    option.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

/** Change Access Denied Route Configuration */
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = new PathString("/Administration/AccessDenied");
});

/** Set Password Reset Token Time */
builder.Services.Configure<DataProtectionTokenProviderOptions>(option =>
{
    option.TokenLifespan = TimeSpan.FromHours(5);
});

/** Custom Email Token LifeSpan */
builder.Services.Configure<CustomEmailConfirmationTokenProviderOptions>(option =>
{
    option.TokenLifespan = TimeSpan.FromDays(3);
});

builder.Services.AddControllersWithViews(config =>
{
    /** Cofigure Authorization Polic where user is authorize or not */
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

/** External Login Configuration */
builder.Services.AddAuthentication()
    .AddGoogle(option =>
    {
        option.ClientId = "104533418000-f1842ur4s3ossfilvda5n5050bqhf75i.apps.googleusercontent.com";
        option.ClientSecret = "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";
    })
    .AddFacebook(option =>
    {
        option.ClientId = "727053895344949";
        option.ClientSecret = "8e3cf1b5c9f6b6db515898ed42f9ce65";
    });

/** Configure Nlog Setting */
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

/** Add Claim Policy */
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));

    /** Custom Policy Using RequireAssertion */
    //option.AddPolicy("EditRolePolicy",
    //    policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

    option.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
});

builder.Services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
/** add Encryption Configuration */
builder.Services.AddSingleton<DataProtectionPurposeStrings>();


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