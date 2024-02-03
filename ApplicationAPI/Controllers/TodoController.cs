using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Web;

namespace ApplicationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private const string _connectionString = "";

        private readonly ITokenAcquisition _tokenAcquisition;

        public TodoController(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            IEnumerable<Todo> todos;
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://database.windows.net/user_impersonation" });
            using (var conn = new SqlConnection(_connectionString))
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