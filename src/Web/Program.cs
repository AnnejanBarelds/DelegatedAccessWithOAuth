using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Refit;
using Web.Configuration;
using Web.Data;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddOptions<BackendServiceOptions>().BindConfiguration(typeof(BackendServiceOptions).Name);
            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi(new[] { $"api://{builder.Configuration["BackendServiceOptions:ClientId"]}/.default" })
                .AddInMemoryTokenCaches();
            builder.Services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();

            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
            });

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();
            builder.Services.AddScoped<BackendServiceHandler>();
            builder.Services.AddRefitClient<IBackendService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7206"))
                .AddHttpMessageHandler<BackendServiceHandler>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}