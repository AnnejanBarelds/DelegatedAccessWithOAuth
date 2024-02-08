using BackendService.Configuration;
using Dapper;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace BackendService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private const string Scope = "https://database.windows.net/user_impersonation";

        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly SqlOptions _sqlOptions;

        public TodoController(ITokenAcquisition tokenAcquisition, IOptions<SqlOptions> sqlOptions)
        {
            _tokenAcquisition = tokenAcquisition;
            _sqlOptions = sqlOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            IEnumerable<Todo> todos;
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { Scope });
            using (var conn = new SqlConnection(_sqlOptions.ConnectionString))
            {
                conn.AccessToken = token;
                todos = await conn.QueryAsync<Todo>("select * from tasks");
            }
            return Ok(new TodoResult
            {
                Todos = todos,
                Token = token
            });
        }
    }
}