using Refit;

namespace Web.Data
{
    public interface IBackendService
    {
        [Get("/WeatherForecast")]
        public Task<WeatherForecast[]> GetForecast(DateTime startDate);

        [Get("/Todo")]
        public Task<TodoResult> GetTodos();

        [Get("/Blob")]
        public Task<BlobResult> GetBlob();

        [Get("/Graph")]
        public Task<ProfileResult> GetProfile();

        [Get("/Graph/usercount")]
        public Task<UserCountResult> GetUserCount();
    }
}
