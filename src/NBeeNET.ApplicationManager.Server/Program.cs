using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NBeeNET.ApplicationManager.Server
{
    public class Program
    {
        private static string port = "5000";
        public static void Main(string[] args)
        {
            foreach (var item in args)
            {
                string[] strs = item.Split(":");
                if (strs[0] == "port")
                    port = strs[1];
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:"+port);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
