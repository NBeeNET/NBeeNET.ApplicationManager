using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NBeeNET.ApplicationManager.Server.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet("{appName}/{key?}")]
        public IActionResult Get(string appName, string key = "dev")
        {
            if (string.IsNullOrEmpty(key))
                key = "dev";

            string path = AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Configuration/" + key + ".json";

            if (!System.IO.File.Exists(path))
                return BadRequest("配置文件不存在！");

            string json = System.IO.File.ReadAllText(path);
            
            if(key=="prod")
                return Ok(RSAEncrypt(prod_public_key, json));
            else
                return Ok(RSAEncrypt(public_key, json));
        }

        [HttpGet]
        public IActionResult GetUrl(string appName, string key = "dev")
        {
            return Get(appName, key);
        }

        [Authorize]
        [HttpGet("Admin/{appName}/{key?}")]
        public IActionResult GetJson(string appName, string key = "dev")
        {
            if (string.IsNullOrEmpty(key))
                key = "dev";

            string path = AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Configuration/" + key + ".json";

            if (!System.IO.File.Exists(path))
                return BadRequest("配置文件不存在！");

            string json = System.IO.File.ReadAllText(path);
            
            return Ok(json);
        }

        [Authorize]
        [HttpPost("Admin/{appName}/{key?}")]
        public IActionResult Post(string appName, [FromBody]string value, string key = "dev")
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Configuration/" + key + ".json";
            if (System.IO.File.Exists(path))
                return BadRequest("配置文件已存在！");

            System.IO.File.WriteAllText(path, value);

            return Ok();
        }

        //[Authorize]
        //[Consumes("multipart/form-data")]
        //[HttpPost("Admin/{appName}")]
        //public IActionResult Post(string appName, IFormFileCollection files)
        //{
        //    if (files.Count <= 0)
        //        return BadRequest();

        //    foreach (var item in files)
        //    {
        //        string path = AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Configuration/" + item.FileName;
        //        if (System.IO.File.Exists(path))
        //            System.IO.File.Copy(path, path + "." + System.DateTime.Now.Ticks.ToString() + ".bak");

        //        using (var stream = System.IO.File.Create(path))
        //        {
        //            item.CopyToAsync(stream);
        //        }
        //    }

        //    return Ok();
        //}

        [Authorize]
        [Consumes("multipart/form-data")]
        [HttpPost("Admin/{appName}")]
        public IActionResult Post(string appName, IFormFile file)
        {
            if (file == null)
                return BadRequest();

            string path = AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Configuration/" + file.FileName;
            if (System.IO.File.Exists(path))
                System.IO.File.Copy(path, path + "." + System.DateTime.Now.Ticks.ToString() + ".bak");

            using (var stream = System.IO.File.Create(path))
            {
                file.CopyToAsync(stream);
            }

            return Ok();
        }

        

        [Authorize]
        [HttpPut("Admin/{appName}/{key?}")]
        public IActionResult Put(string appName, [FromBody]string value, string key = "dev")
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Configuration/" + key + ".json";
            if (!System.IO.File.Exists(path))
                return BadRequest("配置文件不存在！");

            System.IO.File.Copy(path, path + "." + System.DateTime.Now.Ticks.ToString() + ".bak");

            System.IO.File.WriteAllText(path, value);

            return Ok();
        }

        [Authorize]
        [HttpDelete("Admin/{appName}/{key?}")]
        public IActionResult Delete(string appName, string key = "dev")
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"/Application/" + appName + "/Configuration/" + key + ".json";
            if (!System.IO.File.Exists(path))
                return BadRequest("配置文件不存在！");

            System.IO.File.Copy(path, path + "." + System.DateTime.Now.Ticks.ToString() + ".bak");

            System.IO.File.Delete(path);

            return Ok();
        }

        private string prod_public_key
        {
            get
            {
                string pkey = "<RSAKeyValue><Modulus>0c/Rsty0I2ifr/MD/F7sCCNbCP5aHlK4vA1wBzj5inhGh1f7LAGJcTk+nzlaMUOmQiXqUihUy5g0ESpgypabTAxb5NVZbBJ24X0zDejgM8NQnRPYdLzOWcT6iha9E14p3Akc71sIOAJ5Tz3TIZDORriffqFPWVHiTjwHewwdJSAFP8S2ujM04TdtXuN8D6mslJcc1PMC6SqnsyD6F5cEzYbMDDUep4a9GSXBV6+0vO5PmHiiF1bKDGuNajXcb9attvOFINkP55ZJzY9Bq1VCWxXvqJORL8wOMWsdoGkRj522tj3fWiXi/Zd61qj4ODtm1IK4/yb7nadDwZlymhoZ4Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/prodpublickey.xml"))
                    pkey = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/prodpublickey.xml");
                return pkey;
            }
        }

        private string public_key
        {
            get
            {
                string pkey = "<RSAKeyValue><Modulus>0c/Rsty0I2ifr/MD/F7sCCNbCP5aHlK4vA1wBzj5inhGh1f7LAGJcTk+nzlaMUOmQiXqUihUy5g0ESpgypabTAxb5NVZbBJ24X0zDejgM8NQnRPYdLzOWcT6iha9E14p3Akc71sIOAJ5Tz3TIZDORriffqFPWVHiTjwHewwdJSAFP8S2ujM04TdtXuN8D6mslJcc1PMC6SqnsyD6F5cEzYbMDDUep4a9GSXBV6+0vO5PmHiiF1bKDGuNajXcb9attvOFINkP55ZJzY9Bq1VCWxXvqJORL8wOMWsdoGkRj522tj3fWiXi/Zd61qj4ODtm1IK4/yb7nadDwZlymhoZ4Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/publickey.xml"))
                    pkey = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/publickey.xml");
                return pkey;
            }
        }

        private string prod_private_key
        {
            get
            {
                string pkey = "<RSAKeyValue><Modulus>0c/Rsty0I2ifr/MD/F7sCCNbCP5aHlK4vA1wBzj5inhGh1f7LAGJcTk+nzlaMUOmQiXqUihUy5g0ESpgypabTAxb5NVZbBJ24X0zDejgM8NQnRPYdLzOWcT6iha9E14p3Akc71sIOAJ5Tz3TIZDORriffqFPWVHiTjwHewwdJSAFP8S2ujM04TdtXuN8D6mslJcc1PMC6SqnsyD6F5cEzYbMDDUep4a9GSXBV6+0vO5PmHiiF1bKDGuNajXcb9attvOFINkP55ZJzY9Bq1VCWxXvqJORL8wOMWsdoGkRj522tj3fWiXi/Zd61qj4ODtm1IK4/yb7nadDwZlymhoZ4Q==</Modulus><Exponent>AQAB</Exponent><P>5I+0pJQVT3mIjtANErGiDiXhUTPTaWrC9DspWpxMLXOmntumXJ9aSzAOc7ppuzazpT0IKJLJb8B7Yq6wxfK4UFXIswV7Axv0WvYhLlvVuKgCAXjSLoDB1Eo/l1Ts+0Y7CFnZpTKzsAcZnevIm5XwfmdHhjvQjjOykd9YykRXj5s=</P><Q>6v/j25nHJLL8ZXMFwICnXc5QVVozZRoEIrLWFewDDHAJE51ms46ot5fBL4mWKJZrczoMLxQGDRTS2xb4Qaj77LhHnOdX9h/Hgk2NTXaneyrWgpT4FGS02sHVg6rijCuzLk/NgZ/saga7XIt0ZvbxRwrfulZ4v232VGHTJJ1EWjM=</Q><DP>yhESj34r6Pt1c20UYbaRoxhyPywmeEhe2zWCtg0AfB6G7GMcjT6EwXxXCpF/8HfIEwoGMHi1hgsSCoiO2DJiKQPuT/dEbddFDTYU17txE7PVMh/zEhHbSyfpWI1Ihk3s2tE0zgwpODLOvwM5c/P3jokYfuBa3Z1u/fALu6b4Eok=</DP><DQ>2ko6kYml98wfMemHhUq8rSFxc+dm6FojT0wKG7DxIAOOGt9SQCabSHc553uJtUgMpatdF/okqNAYEZWjo/JhmYGQXAp1pdXdUIfTYwp+BqZDPmc8jk0BxlbZjAYXjRcmyTUt82PnipGYRx3vUZm/hDtwmTmrzj5J3BZGuLMxhAs=</DQ><InverseQ>TsnOUVJdZ5OMNfUdK97cKkp1YiE9j5S+yuphukwrQaxa2ULutUzckQvptzkcyf6smhTstJQGm2S2mNzUbQG/fhUNB0Aznz8XmseO2QTvLpC3PORL9Vb8qqnXO/9gBUVvqiG0MlNoz1ydbYRbSjt76V+H2LWjTqBl8GfAihoipfE=</InverseQ><D>q0RCQAgK//BykdUExujRMV9y+2dtNt19sPwjG7I1eQPvRqHaBTUH9aWt6ZmOdzJqsAp1OTV06nDG59A5DIZeJHjeNnC1uxNjeXOSzmLvSTHBR0eg5MIOi5bQkRI4q2utLdE+jmA32Eikr7ue8gJbb71bbKnOtPGzkvtLe/UKgXckije9G1fhnF8nnz336iWc+5F3ODOuHNhtn41d1djb2YBdfxMupaCS4bwzdwyIuIjkztwoK602qavCjGbtgHDQl/xIPLxPm18ERJR3zCepzSqeAM8cmm8P6NFJ+cD7znmpRB1waigaOyUBV2MwOfxDUZ+rMOrQECi2OJSs9lgDdQ==</D></RSAKeyValue>";
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/prodprivatekey.xml"))
                    pkey = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/prodprivatekey.xml");
                return pkey;
            }
        }

        private string private_key
        {
            get
            {
                string pkey = "<RSAKeyValue><Modulus>0c/Rsty0I2ifr/MD/F7sCCNbCP5aHlK4vA1wBzj5inhGh1f7LAGJcTk+nzlaMUOmQiXqUihUy5g0ESpgypabTAxb5NVZbBJ24X0zDejgM8NQnRPYdLzOWcT6iha9E14p3Akc71sIOAJ5Tz3TIZDORriffqFPWVHiTjwHewwdJSAFP8S2ujM04TdtXuN8D6mslJcc1PMC6SqnsyD6F5cEzYbMDDUep4a9GSXBV6+0vO5PmHiiF1bKDGuNajXcb9attvOFINkP55ZJzY9Bq1VCWxXvqJORL8wOMWsdoGkRj522tj3fWiXi/Zd61qj4ODtm1IK4/yb7nadDwZlymhoZ4Q==</Modulus><Exponent>AQAB</Exponent><P>5I+0pJQVT3mIjtANErGiDiXhUTPTaWrC9DspWpxMLXOmntumXJ9aSzAOc7ppuzazpT0IKJLJb8B7Yq6wxfK4UFXIswV7Axv0WvYhLlvVuKgCAXjSLoDB1Eo/l1Ts+0Y7CFnZpTKzsAcZnevIm5XwfmdHhjvQjjOykd9YykRXj5s=</P><Q>6v/j25nHJLL8ZXMFwICnXc5QVVozZRoEIrLWFewDDHAJE51ms46ot5fBL4mWKJZrczoMLxQGDRTS2xb4Qaj77LhHnOdX9h/Hgk2NTXaneyrWgpT4FGS02sHVg6rijCuzLk/NgZ/saga7XIt0ZvbxRwrfulZ4v232VGHTJJ1EWjM=</Q><DP>yhESj34r6Pt1c20UYbaRoxhyPywmeEhe2zWCtg0AfB6G7GMcjT6EwXxXCpF/8HfIEwoGMHi1hgsSCoiO2DJiKQPuT/dEbddFDTYU17txE7PVMh/zEhHbSyfpWI1Ihk3s2tE0zgwpODLOvwM5c/P3jokYfuBa3Z1u/fALu6b4Eok=</DP><DQ>2ko6kYml98wfMemHhUq8rSFxc+dm6FojT0wKG7DxIAOOGt9SQCabSHc553uJtUgMpatdF/okqNAYEZWjo/JhmYGQXAp1pdXdUIfTYwp+BqZDPmc8jk0BxlbZjAYXjRcmyTUt82PnipGYRx3vUZm/hDtwmTmrzj5J3BZGuLMxhAs=</DQ><InverseQ>TsnOUVJdZ5OMNfUdK97cKkp1YiE9j5S+yuphukwrQaxa2ULutUzckQvptzkcyf6smhTstJQGm2S2mNzUbQG/fhUNB0Aznz8XmseO2QTvLpC3PORL9Vb8qqnXO/9gBUVvqiG0MlNoz1ydbYRbSjt76V+H2LWjTqBl8GfAihoipfE=</InverseQ><D>q0RCQAgK//BykdUExujRMV9y+2dtNt19sPwjG7I1eQPvRqHaBTUH9aWt6ZmOdzJqsAp1OTV06nDG59A5DIZeJHjeNnC1uxNjeXOSzmLvSTHBR0eg5MIOi5bQkRI4q2utLdE+jmA32Eikr7ue8gJbb71bbKnOtPGzkvtLe/UKgXckije9G1fhnF8nnz336iWc+5F3ODOuHNhtn41d1djb2YBdfxMupaCS4bwzdwyIuIjkztwoK602qavCjGbtgHDQl/xIPLxPm18ERJR3zCepzSqeAM8cmm8P6NFJ+cD7znmpRB1waigaOyUBV2MwOfxDUZ+rMOrQECi2OJSs9lgDdQ==</D></RSAKeyValue>";
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/privatekey.xml"))
                    pkey = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/privatekey.xml");
                return pkey;
            }
        }

        /// <summary>
        /// RSA加密+base64
        /// </summary>
        /// <param name="publickey">公钥</param>
        /// <param name="content">原文</param>
        /// <returns>加密后的密文字符串</returns>
        public static string RSAEncrypt(string publickey, string content)
        {
            //最大文件加密块
            int MAX_ENCRYPT_BLOCK = 245;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            byte[] contentByte = Encoding.UTF8.GetBytes(content);
            int inputLen = contentByte.Length;

            int offSet = 0;
            byte[] cache;
            int i = 0;
            System.IO.MemoryStream aMS = new System.IO.MemoryStream();
            // 对数据分段加密
            while (inputLen - offSet > 0)
            {
                byte[] temp = new byte[MAX_ENCRYPT_BLOCK];
                if (inputLen - offSet > MAX_ENCRYPT_BLOCK)
                {
                    Array.Copy(contentByte, offSet, temp, 0, MAX_ENCRYPT_BLOCK);
                    cache = rsa.Encrypt(temp, false);
                }
                else
                {
                    Array.Copy(contentByte, offSet, temp, 0, inputLen - offSet);
                    cache = rsa.Encrypt(temp, false);
                }
                aMS.Write(cache, 0, cache.Length);
                i++;
                offSet = i * MAX_ENCRYPT_BLOCK;
            }

            cipherbytes = aMS.ToArray();
            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privatekey">私钥</param>
        /// <param name="content">密文（RSA+base64）</param>
        /// <returns>解密后的字符串</returns>
        public static string RSADecrypt(string privatekey, string content)
        {
            //最大文件解密块
            int MAX_DECRYPT_BLOCK = 256;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privatekey);
            byte[] contentByte = Convert.FromBase64String(content);
            int inputLen = contentByte.Length;

            // 对数据分段解密
            int offSet = 0;
            int i = 0;
            byte[] cache;
            System.IO.MemoryStream aMS = new System.IO.MemoryStream();
            while (inputLen - offSet > 0)
            {
                byte[] temp = new byte[MAX_DECRYPT_BLOCK];
                if (inputLen - offSet > MAX_DECRYPT_BLOCK)
                {
                    Array.Copy(contentByte, offSet, temp, 0, MAX_DECRYPT_BLOCK);
                    cache = rsa.Decrypt(temp, false);
                }
                else
                {
                    Array.Copy(contentByte, offSet, temp, 0, inputLen - offSet);
                    cache = rsa.Decrypt(temp, false);
                }
                aMS.Write(cache, 0, cache.Length);
                i++;
                offSet = i * MAX_DECRYPT_BLOCK;
            }
            cipherbytes = aMS.ToArray();

            return Encoding.UTF8.GetString(cipherbytes);
        }

    }
}