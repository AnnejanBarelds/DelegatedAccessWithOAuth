using BackendService.Data;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace BackendService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherApi _weatherApi;
        private string? _lastToken;

        public WeatherController(IWeatherApi weatherApi)
        {
            _weatherApi = weatherApi;
            WeatherApiHandler.TokenAcquired += TokenAcquired;
        }

        private void TokenAcquired(object? sender, string e)
        {
            _lastToken = e;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _weatherApi.GetForecast();
            return Ok(new WeatherForecastResult
            {
                Forecasts = result,
                Token = _lastToken
            });
        }
    }
}
