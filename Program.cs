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
app.UseMvcWithDefaultRoute();

app.Run();