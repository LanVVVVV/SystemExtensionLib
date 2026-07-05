using System.IO;
using System.Reflection;
using SystemExtensionLib.Core;
using UnityEngine;

namespace SystemExtensionLib.Sprites;

internal static class UISpriteLoad
{
    private static readonly Assembly assembly = Assembly.GetExecutingAssembly();

    internal static Sprite? InterfaceExtendedArea { get; set; }

    internal static void LoadSprite()
    {
        InterfaceExtendedArea = GetEmbeddedSprite
            (
            "window_interface_ExtendedArea.png", 
            new Vector4(13, 13, 14, 53)
            );
    }

    private static Sprite? GetEmbeddedSprite(string embeddedResourceName, Vector4? border = null)
    {
        using Stream stream = assembly.GetManifestResourceStream(embeddedResourceName);
        if (stream == null)
        {
            ModEntry.LogError($"Embedded resource not found: {embeddedResourceName}");
            return null;
        }

        byte[] imageData = new byte[stream.Length];
        stream.Read(imageData, 0, imageData.Length);

        Texture2D texture = new Texture2D(1, 1);
        if (texture.LoadImage(imageData))
        {
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);

            if (border.HasValue)
            {
                return Sprite.Create(texture, rect, pivot, 100f,
                    0, SpriteMeshType.FullRect, border.Value
                );
            }
            else
            {
                return Sprite.Create(texture, rect, pivot, 100f
                );
            }
        }
        else
        {
            ModEntry.LogError($"Failed to parse embedded resource: {embeddedResourceName}");
            return null;
        }
    }
}
