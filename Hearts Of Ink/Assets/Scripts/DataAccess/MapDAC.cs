using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    public class MapDAC
    {
        private const string GlobalInfoFile = "/MapDefinitions/_GlobalInfo.json";

        public static MapModel LoadMapInfo(string mapName)
        {
            string mapPath = Application.streamingAssetsPath + "/MapDefinitions/" + mapName  + ".json";

            return JsonCustomUtils<MapModel>.ReadObjectFromFile(mapPath);
        }

        public static GlobalInfo LoadGlobalMapInfo()
        {
            string globalInfoPath = Application.streamingAssetsPath + GlobalInfoFile;

            return JsonCustomUtils<GlobalInfo>.ReadObjectFromFile(globalInfoPath);
        }
    }
}
