using Microsoft.AspNetCore.HttpOverrides;
using AnunumEmployees.Extensions;
using AnunumEmployees.Presentation;
using Contracts;
using LoggerService;
using NLog;

namespace AnunumEmployees;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

        // Add services to the container.

        builder.Services.ConfigureCors();
        builder.Services.ConfigureIISIntegration();
        builder.Services.ConfigureSqlContext(builder.Configuration);
        builder.Services.ConfigureLoggerService();
        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.ConfigureRepositoryManager();
        builder.Services.ConfigureServiceManager();
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(AssemblyReference).Assembly);

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });

        app.UseCors("CorsPolicy");

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
