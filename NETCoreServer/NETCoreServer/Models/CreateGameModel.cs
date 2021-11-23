using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Models
{
    public class CreateGameModel
    {
        public string name { get; set; }
        public bool isPublic { get; set; }
        public byte mapId { get; internal set; }
    }
}
