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

        public static MapModel LoadMapInfo(short mapId)
        {
            List<MapModel> availableMaps = GetAvailableMaps();

            return availableMaps.Find(map => map.MapId == mapId);
        }

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

        private static List<MapModel> GetAvailableMaps()
        {
            string directory = Application.streamingAssetsPath + "/MapDefinitions";
            string[] files = Directory.GetFiles(directory, "*.json");
            List<MapModel> mapModels = new List<MapModel>();

            foreach (string file in files)
            {
                mapModels.Add(LoadMapInfo(file));
            }

            return mapModels;
        }

        public static List<MapModel> GetAvailableMaps(bool isForMultiplayer)
        {
            List<MapModel> mapModels = GetAvailableMaps();

            if (isForMultiplayer)
            {
                return mapModels.Where(map => map.AvailableForMultiplayer).ToList();
            }
            else
            {
                return mapModels.Where(map => map.AvailableForSingleplayer).ToList();
            }
        }

        public static Sprite LoadMapSprite(string spritePath)
        {
            Sprite result;
            byte[] imageData;
            Texture2D texture = new Texture2D(2, 2);
            Rect rect;
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            string fullPath;

            fullPath = Application.streamingAssetsPath;
            fullPath += spritePath.StartsWith("/") ? spritePath : "/" + spritePath;
            imageData = File.ReadAllBytes(fullPath);
            texture.LoadImage(imageData);
            rect = new Rect(0f, 0f, texture.width, texture.height);

            result = Sprite.Create(texture, rect, pivot, texture.height / 8);

            return result;
        }
    }
}
