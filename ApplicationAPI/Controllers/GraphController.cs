using BackendService.Data;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Identity.Web.Resource;

namespace BackendService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class GraphController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string? _lastToken;

        public GraphController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            GraphApiHandler.TokenAcquired += OnTokenAcquired;
        }

        private void OnTokenAcquired(object? sender, string e)
        {
            _lastToken = e;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var httpClient = _httpClientFactory.CreateClient("Graph");
            var graphClient = new GraphServiceClient(httpClient);
            var profile = await graphClient.Me.GetAsync();
            
            return Ok(new ProfileResult
            {
                Result = profile.DisplayName,
                Token = _lastToken
            });
        }

        [HttpGet("usercount")]
        public async Task<IActionResult> GetAllUsers()
        {
            var httpClient = _httpClientFactory.CreateClient("Graph");
            var graphClient = new GraphServiceClient(httpClient);
            int? userCount = null;
            try
            {
                userCount = await graphClient.Users.Count.GetAsync();
            }
            catch (ODataError ex) when (ex.Error?.Code == "Authorization_RequestDenied")
            {
                return Forbid();
            }
            
            return Ok(new UserCountResult
            {
                Count = userCount,
                Token = _lastToken
            });
        }
    }
}
