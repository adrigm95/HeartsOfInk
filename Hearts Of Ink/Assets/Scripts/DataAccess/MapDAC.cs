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
        // RGMD = RawGen Map Definition
        // RGMH = RawGen Map Header
        private const string RGMDFormat = ".rgmd";
        private const string RGMHFormat = ".rgmh";
        private const string RGMDPattern = "*.rgmd";
        private const string RGMHPattern = "*.rgmh";
        private const string PNGPattern = "*.png";
        private const string JPGPattern = "*.jpg";
        private const string JsonFormat = ".json";
        private const string JsonFormatPattern = "*.json";
        private const string GlobalInfoFile = "/_GlobalInfo.json";
        private const string MapDefinitionsPath = "/MapDefinitions/";

        public static MapModelHeader LoadMapHeader(string filename)
        {
            MapModelHeader result;
            string mapPath;

            try
            {
                if (filename.EndsWith(RGMHFormat))
                {
                    mapPath = filename;
                }
                else
                {
                    mapPath = Application.streamingAssetsPath + MapDefinitionsPath + filename + RGMHFormat;
                }

                result = JsonCustomUtils<MapModelHeader>.ReadObjectFromFile(mapPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error trying to load header map '{filename}': {ex.Message}");
                throw;
            }

            return result;
        }

        public static MapModel LoadMapInfoById(string mapId)
        {
            List<MapModelHeader> availableMaps = GetAvailableMaps();
            MapModelHeader wantedMap = availableMaps.Find(map => map.MapId == mapId);

            return LoadMapInfoByName(wantedMap.DefinitionName);
        }

        public static MapModel LoadMapInfoByName(string mapName)
        {
            MapModel result;
            string mapPath;

            try
            {
                if (mapName.EndsWith(RGMDFormat))
                {
                    mapPath = mapName;
                }
                else
                {
                    mapPath = Application.streamingAssetsPath + MapDefinitionsPath + mapName + RGMDFormat;
                }

                result = JsonCustomUtils<MapModel>.ReadObjectFromFile(mapPath);
                result.DefinitionName = mapName;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error trying to load map '{mapName}': {ex.Message}");
                throw;
            }

            return result;
        }

        public static GlobalInfo LoadGlobalMapInfo()
        {
            string globalInfoPath = Application.streamingAssetsPath + GlobalInfoFile;

            return JsonCustomUtils<GlobalInfo>.ReadObjectFromFile(globalInfoPath);
        }

        public static List<MapModelHeader> GetAvailableMaps()
        {
            string directory = Application.streamingAssetsPath + "/MapDefinitions";
            string[] files = Directory.GetFiles(directory, RGMHPattern);
            List<MapModelHeader> mapModels = new List<MapModelHeader>();

            foreach (string file in files)
            {
                mapModels.Add(LoadMapHeader(file));
            }

            return mapModels;
        }

        public static List<MapModelHeader> GetAvailableMaps(bool isForMultiplayer)
        {
            List<MapModelHeader> mapModels = GetAvailableMaps();

            if (isForMultiplayer)
            {
                return mapModels.Where(map => map.AvailableForMultiplayer).ToList();
            }
            else
            {
                return mapModels.Where(map => map.AvailableForSingleplayer).ToList();
            }
        }

        public static List<string> GetAvailableSprites()
        {
            string directory = Application.streamingAssetsPath + "/MapSprites/";
            string[] files = Directory.GetFiles(directory, PNGPattern);

            return files.ToList().Select(file => file.Split('/').Last()).ToList();
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

        public static void SaveMapDefinition(MapModel mapModel)
        {
            string path = null;

            try
            {
                path = Application.streamingAssetsPath + MapDefinitionsPath + mapModel.DefinitionName + RGMDFormat;
                JsonCustomUtils<MapModel>.SaveObjectIntoFile(mapModel, path);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error trying to saving map '{path}': {ex.Message}");
                throw;
            }
        }

        public static void SaveMapHeader(MapModelHeader mapHeaderModel)
        {
            string path = null;

            try
            {
                path = Application.streamingAssetsPath + MapDefinitionsPath + mapHeaderModel.DefinitionName + RGMHFormat;
                JsonCustomUtils<MapModelHeader>.SaveObjectIntoFile(mapHeaderModel, path);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error trying to saving map header '{path}': {ex.Message}");
                throw;
            }
        }
    }
}
