using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Models
{
    public class BasicGameInfo
    {
        public string name { get; set; }
        public string hostIp { get; set; }
        public string gameKey { get; set; }
        public byte playersInside { get; set; }
        public byte mapId { get; set; }
        public short ping { get; set; }
        public bool isPublic { get; set; }
    }
}
