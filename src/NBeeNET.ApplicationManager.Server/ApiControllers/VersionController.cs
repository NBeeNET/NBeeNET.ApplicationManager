using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NBeeNET.ApplicationManager.Server.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string appName)
        {
            if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Version/Current.json"))
                return BadRequest("配置文件不存在！");
            string currentJson = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Version/Current.json");
            Hashtable hashtable = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(currentJson);
            return Ok(hashtable);
        }
    }
}