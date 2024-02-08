using DTO;
using Refit;

namespace BackendService.Data
{
    public interface IWeatherApi
    {
        [Get("/WeatherForecast")]
        public Task<WeatherForecast[]> GetForecast();
    }
}
