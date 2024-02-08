using BackendService.Configuration;
using BackendService.Data;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Refit;

namespace BackendService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();

            builder.Services.AddScoped<IAzureClientFactory, AzureClientFactory>();
            builder.Services.AddScoped<TokenCredential, OnBehalfOfTokenCredential>();

            builder.Services.AddHttpClient("Graph")
                .AddHttpMessageHandler<GraphApiHandler>();
            builder.Services.AddScoped<GraphApiHandler>();

            builder.Services.AddScoped<WeatherApiHandler>();
            builder.Services.AddRefitClient<IWeatherApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7016"))
                .AddHttpMessageHandler<WeatherApiHandler>();

            builder.Services.AddOptions<SqlOptions>().BindConfiguration(typeof(SqlOptions).Name);
            builder.Services.AddOptions<StorageOptions>().BindConfiguration(typeof(StorageOptions).Name);
            builder.Services.AddOptions<WeatherApiOptions>().BindConfiguration(typeof(WeatherApiOptions).Name);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}