using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Models
{
    public class ServerGameInfo
    {
        public BasicGameInfo basicGameInfo;
        public DateTime lastNoticeFromHost;

        public ServerGameInfo(BasicGameInfo newGame)
        {
            this.basicGameInfo = newGame;
            lastNoticeFromHost = DateTime.UtcNow;
        }
    }
}
