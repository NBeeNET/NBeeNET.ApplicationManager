using System;
using System.Collections;
using System.Threading;

namespace NBeeNET.ApplicationManager.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(3000);

            Hashtable hashtable = new Agent.Configuration("https://api.waijiaoyi.com/ApplicationManager/").Get("SceneClass_AI","");
            foreach (var item in hashtable.Keys)
            {
                Console.WriteLine(item + ":" + hashtable[item]);
            }
            

            hashtable = new Agent.Version("https://api.waijiaoyi.com/ApplicationManager/").Get("SceneClass_AI");

            foreach (var item in hashtable.Keys)
            {
                Console.WriteLine(item + ":" + hashtable[item]);
            }

            Console.ReadLine();
        }
    }
}
