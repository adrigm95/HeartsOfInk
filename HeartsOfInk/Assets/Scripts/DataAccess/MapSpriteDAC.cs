using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    public class MapSpriteDAC
    {
        public static Sprite LoadMapSprite(string spriteName)
        {
            Sprite result;
            byte[] imageData;
            Texture2D texture = new Texture2D(2, 2);
            Rect rect;
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            string fullPath;

            fullPath = Application.persistentDataPath;
            if (!fullPath.EndsWith("/") && !fullPath.EndsWith("\\"))
            {
                fullPath += "/";
            }

            fullPath += "MapSprites";
            fullPath += spriteName.StartsWith("/") ? spriteName : "/" + spriteName;
            imageData = File.ReadAllBytes(fullPath);
            texture.LoadImage(imageData);
            rect = new Rect(0f, 0f, texture.width, texture.height);

            result = Sprite.Create(texture, rect, pivot, texture.height / 8);

            return result;
        }

        public static void SaveMapSprite(string rootPath, string spriteName, string sprite)
        {
            byte[] allBytes = null;
            string fullPath;

            try
            {
                fullPath = rootPath;

                if (!fullPath.EndsWith("/") && !fullPath.EndsWith("\\"))
                {
                    fullPath += "/";
                }

                fullPath += "MapSprites/";
                Directory.CreateDirectory(fullPath);

                fullPath += spriteName.StartsWith("/") ? spriteName : "/" + spriteName;
                allBytes = Convert.FromBase64String(sprite);
                File.WriteAllBytes(fullPath, allBytes);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
