namespace DTO
{
    public class WeatherForecastResult
    {
        public IEnumerable<WeatherForecast> Forecasts { get; set; }

        public string Token { get; set; }
    }
}
