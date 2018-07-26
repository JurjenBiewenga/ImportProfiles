using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

public static class AssetCreationFunctions
{
    public static Object CreateModel(string path)
    {
        TextAsset asset = new TextAsset();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    public static Object CreateTexture(string path)
    {
        Texture2D asset = new Texture2D(4, 4);
        File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Application.dataPath), path), asset.EncodeToPNG());
        return asset;
    }
}