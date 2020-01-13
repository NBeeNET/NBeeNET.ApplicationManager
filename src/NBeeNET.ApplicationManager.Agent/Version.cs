using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NBeeNET.ApplicationManager.Agent
{
    public class Version
    {
        private string mHost = "http://localhost:5000/api/";

        /// <summary>
        /// 版本服务地址
        /// </summary>
        /// <param name="host"></param>
        public Version(string host)
        {
            if (!string.IsNullOrEmpty(host))
                mHost = host;
        }

        /// <summary>
        /// 获取当前版本
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public Hashtable Get(string appName)
        {
            var client = new RestSharp.RestClient(mHost);
            var requestGet = new RestRequest("Version?appName=" + appName, Method.GET);
            IRestResponse response = client.Execute(requestGet);

            string contentGet = response.Content;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(contentGet);
        }
    }
}
