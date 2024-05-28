using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Demo_Project.Config;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Demo_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyFabricTokenService : ControllerBase
    {
        [HttpPost]
        public async Task<String> PostAsync()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);

            var values = new Dictionary<string, string>
             {
                  { "appSecret", Config.Config.appSecrete }
             };
            var content = new FormUrlEncodedContent(values);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-APP-Key", Config.Config.fabricAppId);

            var response = await client.PostAsync(Config.Config.baseUrl + "/payment/v1/token/", content);

            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
