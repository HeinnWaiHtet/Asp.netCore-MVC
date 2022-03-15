using Asp.netCore_MVC.Models;

var builder = WebApplication.CreateBuilder(args);

/** 
 * Add services to the container.
 */
/** Add MVC service */
builder.Services.AddMvc(options =>
{
    /** check end point routing */
    options.EnableEndpointRouting = false;
});

builder.Services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();


/**
 * Configure the HTTP request pipeline.
 */
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

/** Check Development mode and add Developer Exception */
if (app.Environment.IsDevelopment())
{
    DeveloperExceptionPageOptions options = new DeveloperExceptionPageOptions();
    options.SourceCodeLineCount = 10;
    app.UseDeveloperExceptionPage(options);
}

/** add static file middleware for connect under wwwroot */
app.UseStaticFiles();

/** Configure MVC controller with default routes */
//app.UseMvcWithDefaultRoute();

/** Configure Conventional Routing With Default Routes */
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseMvc(route =>
{
    route.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
});

app.Run();