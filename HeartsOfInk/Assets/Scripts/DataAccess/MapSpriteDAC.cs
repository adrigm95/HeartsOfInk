using System.IO;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    public class MapSpriteDAC
    {
        public static Sprite LoadMapSprite(string spritePath)
        {
            Sprite result;
            byte[] imageData;
            Texture2D texture = new Texture2D(2, 2);
            Rect rect;
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            string fullPath;

            fullPath = Application.persistentDataPath;
            fullPath += spritePath.StartsWith("/") ? spritePath : "/" + spritePath;
            imageData = File.ReadAllBytes(fullPath);
            texture.LoadImage(imageData);
            rect = new Rect(0f, 0f, texture.width, texture.height);

            result = Sprite.Create(texture, rect, pivot, texture.height / 8);

            return result;
        }
    }
}
