using ContactsManagement.UI.StartupExtensions;
using Microsoft.IdentityModel.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
        .ReadFrom.Services(services); //read out current app's services and make them available to serilog
} );

//Set up the configuration and services using extension method 
builder.Services.ConfigureServices(configuration:builder.Configuration , webHostEnvironment:builder.Environment);


var app = builder.Build();
app.UseHttpsRedirection();
app.UseHsts();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    IdentityModelEventSource.ShowPII = true;
}
else
{
    // app.UseCustomExceptionHandlingMiddleware();
    app.UseExceptionHandler("/Error");
}


app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.Run();

public partial class Program { }