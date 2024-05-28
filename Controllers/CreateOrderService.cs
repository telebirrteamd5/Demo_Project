using Demo_Project.Config;
using Demo_Project.Models;
using Demo_Project.Service;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Demo_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateOrderService : ControllerBase
    {
        [HttpPost]
        public async Task<string> PostAsync([FromBody] Request request)
        {
            Console.WriteLine(request);
            
            string title = request.Title;
            string amount = request.Amount;
            Console.WriteLine("Title "+title + " Amount "+amount);      
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ApplyFabricTokenService applyFabricTokenService = new ApplyFabricTokenService();
            string applyFabricToken = await applyFabricTokenService.PostAsync();
            dynamic data = JObject.Parse(applyFabricToken);
            string token = data.token;
            client.DefaultRequestHeaders.Add("X-APP-Key", Config.Config.fabricAppId);
            client.DefaultRequestHeaders.Add("Authorization", token);
            var values = createRequestObject(title, amount);
            Console.WriteLine(values);
            var jsonValues = JsonConvert.SerializeObject(values);
            var content = new StringContent(jsonValues, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Config.Config.baseUrl + "/payment/v1/merchant/preOrder", content);
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic responseData = JObject.Parse(responseString);
            string prePayId = responseData.biz_content.prepay_id;
            return createRawRequest(prePayId);

        }
        public static Dictionary<String, object> createRequestObject(string title, string amount)
        {
            var biz = new Dictionary<string, string>{
                    { "trans_currency","ETB"},
                    { "total_amount",amount},
                    { "merch_order_id",Service.Tools.createTimeStamp()},
                    { "appid",Config.Config.merchantAppId},
                    { "merch_code",Config.Config.merchantCode},
                    { "timeout_express","120m"},
                    { "trade_type","InApp"},
                    { "notify_url","https://www.google.com"},
                    { "title",title},
                    { "business_type","BuyGoods"},
                    { "payee_identifier_type","04"},
                    { "payee_identifier",Config.Config.merchantCode},
                    { "payee_type","5000"}
                };
            var req = new Dictionary<string, object>
            {
                {"nonce_str",Service.Tools.createNonceStr()},
                {"biz_content" , biz },
                {"method","payment.preorder"},
                {"version", "1.0"},
                {"timestamp",Service.Tools.createTimeStamp() }
        }; 
            req.Add("sign", Service.Tools.sign(req));
            req.Add("sign_type", "SHA256WithRSA");
            return req;
        }

        public static string createRawRequest(string prepayId)
        {
            var maps = new Dictionary<string, string>
            {
                {"appid",Config.Config.merchantAppId },
                {"merch_code",Config.Config.merchantCode },
                {"nonce_str",Tools.createNonceStr() },
                {"prepay_id",prepayId },
                {"timestamp",Tools.createTimeStamp() },
                {"sign_type","SHA256withRSA" }

            };
            string[] raw = { };
            Dictionary<string, string>.KeyCollection keys = maps.Keys;
            foreach (string key in keys)
            {
                raw = raw.Append(key + "=" + maps[key]).ToArray();

            }
            Array.Sort(raw);
            string sorted = string.Join("&", raw);

            return sorted;
        }
}
}
