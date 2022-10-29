using TaskTracker.Models;
using TaskTracker.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using KafkaTransmitter.Services;
using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using System.Net;
using Microsoft.Extensions.Options;

namespace TaskTracker;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // получаем строку подключения из файла конфигурации
        string connection = Configuration.GetConnectionString("TaskTrackerDB");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));

        services.AddRazorPages();
        // configure strongly typed settings object
        services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
        services.Configure<ConsumerConfig>(Configuration.GetSection("ConsumerConfig"));
        services.Configure<ProducerConfig>(Configuration.GetSection("ProducerConfig"));

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/users/login";
                options.AccessDeniedPath = "/users/denied";
            });

        services.AddAuthorization();

        // configure DI for application services
        services.AddScoped<StreamEventHandlerService>();
        services.AddHostedService<BackgroundConsumerService>(sp => {

            BackgroundConsumerTopicOptions options = new();
            options.Handlers.Add("accounts-stream", sp => sp.GetRequiredService<StreamEventHandlerService>());

            return new BackgroundConsumerService(sp, options);
        });

        services.AddScoped<IEventProducerService, EventProducerService>();

        services.AddTransient<TaskCreationService>();
        services.AddTransient<TaskCompletionService>();
        services.AddTransient<TasksAssignmentService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (env.IsDevelopment()) 
        {
            app.UseDeveloperExceptionPage();
        } 
        else 
        { 
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseMiddleware<JwtMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }
}
