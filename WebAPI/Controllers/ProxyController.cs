using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace WebAPI.Controllers
{
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private async Task<JsonResult> ProxyRequest(HttpMethod method, string url)
        {
            HttpRequestMessage message = new HttpRequestMessage(method,
                $"http://localhost:4567/{url}");

            string apiKey = Request.Headers["API_KEY"];
            if (apiKey != null)
            {
                if (apiKey.Length > 0)
                {
                    message.Headers.Add("API_KEY", apiKey);
                }
            }

            message.Content = new StreamContent(Request.Body);

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(message);
            Response.StatusCode = (int) response.StatusCode;

            string responseContent = await response.Content.ReadAsStringAsync();

            if (responseContent.Length <= 0) return new JsonResult("ok");
            
            JObject jsonResponse = JObject.Parse(responseContent);
            return new JsonResult(jsonResponse);
        }

        [HttpPost("{*url}")]
        public async Task<JsonResult> ForwardPost(string url)
        {
            return await ProxyRequest(HttpMethod.Post, url);
        }

        [HttpGet("{*url}")]
        public async Task<JsonResult> Forward(string url)
        {
            return await ProxyRequest(HttpMethod.Get, url);
        }
    }
}