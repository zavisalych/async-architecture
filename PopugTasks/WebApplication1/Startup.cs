using AuthN.Entities;
using AuthN.Models;
using AuthN.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json.Serialization;
using KafkaTransmitter.Services;

namespace AuthN;

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
        string connection = Configuration.GetConnectionString("AuthNDB");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));

        services.AddRazorPages();
        services.AddControllersWithViews().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // configure strongly typed settings object
        services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
        services.Configure<ProducerConfig>(Configuration.GetSection("ProducerConfig"));

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/users/login";
                options.AccessDeniedPath = "/users/denied";
            });

        services.AddAuthorization();

        // configure DI for application services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEventProducerService, EventProducerService>();
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

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });

        SeedData.Initialize(app.ApplicationServices);
    }
}
