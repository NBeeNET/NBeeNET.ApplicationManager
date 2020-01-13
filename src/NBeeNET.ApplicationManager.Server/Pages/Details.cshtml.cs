using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NBeeNET.ApplicationManager.Server.Pages
{
    public class DetailsModel : PageModel
    {
        public string mAppName = string.Empty;
        public void OnGet(string appName)
        {
            mAppName = appName;
            ViewData["AppName"] = appName;

            ViewData["ConfigurationList"] = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Configuration/","*.json");

            //ViewData["CurrentVersion"] = System.IO.File.ReadAllText(@"Application/" + appName + "/Version/current.json");

            ViewData["VersionList"] = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Version/","*.json");
        }
    }
}