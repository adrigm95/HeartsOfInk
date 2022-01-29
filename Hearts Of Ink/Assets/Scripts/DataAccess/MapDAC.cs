using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    public class MapDAC
    {
        private const string GlobalInfoFile = "/_GlobalInfo.json";

        public static MapModel LoadMapInfo(string mapName)
        {
            MapModel result;
            string mapPath;

            try
            {
                if (mapName.EndsWith(".json"))
                {
                    mapPath = mapName;
                }
                else
                {
                    mapPath = Application.streamingAssetsPath + "/MapDefinitions/" + mapName + ".json";
                }

                result = JsonCustomUtils<MapModel>.ReadObjectFromFile(mapPath);
                result.DefinitionName = mapName;
            }
            catch (Exception ex)
            {
                Debug.LogError("Error trying to load map '" + mapName + "': " + ex.Message);
                throw;
            }

            return result;
        }

        public static GlobalInfo LoadGlobalMapInfo()
        {
            string globalInfoPath = Application.streamingAssetsPath + GlobalInfoFile;

            return JsonCustomUtils<GlobalInfo>.ReadObjectFromFile(globalInfoPath);
        }

        public static List<MapModel> GetAvailableMaps(bool isForMultiplayer)
        {
            string directory = Application.streamingAssetsPath + "/MapDefinitions";
            string[] files = Directory.GetFiles(directory, "*.json");
            List<MapModel> mapModels = new List<MapModel>();

            foreach (string file in files)
            {
                mapModels.Add(LoadMapInfo(file));
            }

            if (isForMultiplayer)
            {
                return mapModels.Where(map => map.AvailableForMultiplayer).ToList();
            }
            else
            {
                return mapModels.Where(map => map.AvailableForSingleplayer).ToList();
            }
        }
    }
}
