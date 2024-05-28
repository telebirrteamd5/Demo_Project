using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Demo_Project.Models;

namespace Demo_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class index : ControllerBase
    {

        //[HttpPost]
        //public async Task<JsonResult> PostAsync([FromBody] Request request)
        //{
        //    var biz = request;
        //    return JsonResult(new { title = request.Title, amount = biz.Amount });
        //}
    }
}
