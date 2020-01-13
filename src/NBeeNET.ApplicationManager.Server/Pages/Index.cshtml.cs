using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace NBeeNET.ApplicationManager.Server.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            string[] strs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @"Application");
            string[] strss = new string[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                strss[i] = new DirectoryInfo(strs[i]).Name;
            }
            ViewData["ApplicationList"] = strss;

        }
    }
}
