using NETCoreServer.Models.In;
using NETCoreServer.Models.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Models
{
    public class BasicGameInfo
    {
        public string name { get; set; }
        public string gameKey { get; set; }
        public byte playersInside { get; set; }
        public string mapName { get; set; }
        public short ping { get; set; }
        public bool isPublic { get; set; }

        public static BasicGameInfo FromCreateGameService(CreateGameModelIn createGameModelIn, CreateGameModelOut createGameModelOut)
        {
            return new BasicGameInfo()
            {
                name = createGameModelIn.name,
                gameKey = createGameModelOut.gameKey,
                playersInside = 1,
                mapName = createGameModelIn.mapName,
                isPublic = createGameModelIn.isPublic
            };
        }
    }
}
