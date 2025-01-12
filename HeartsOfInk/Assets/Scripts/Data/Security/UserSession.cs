using System;

namespace Assets.Scripts.Data.Security
{
    public class UserSession
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
