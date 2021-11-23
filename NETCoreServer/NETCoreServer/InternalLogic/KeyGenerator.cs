using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.InternalLogic
{
    public class KeyGenerator
    {
        private static Random random = new Random();

        public string BuildKeyForGame(string gameName, int retryCount)
        {
            string base64;
            byte[] bytes;

            bytes = System.Text.Encoding.UTF8.GetBytes(random.Next() + retryCount + gameName);
            base64 = Convert.ToBase64String(bytes);

            return base64.Substring(0, 6).ToUpper();
        } 
    }
}
