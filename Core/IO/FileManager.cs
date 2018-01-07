﻿using System.IO;
using UnityEngine;

namespace Core.IO
{
    public class FileManager
    {
        public static Sprite LoadSpriteFromFile(string path, int width, int height)
        {
            byte[] bytes = File.ReadAllBytes(path);
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);

            texture.filterMode = FilterMode.Trilinear;
            texture.LoadImage(bytes);

            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.0f), 1.0f);
        }
    }
}
