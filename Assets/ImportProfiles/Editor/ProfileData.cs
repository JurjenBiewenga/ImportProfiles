using System.Collections;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ProfileData
{
    public string AssetPath;
    public bool IsDefault;
    public ProfileTypes Type;

    private AssetImporter importer;

    public AssetImporter Importer
    {
        get
        {
            if (importer == null)
                importer = AssetImporter.GetAtPath(AssetPath);
            return importer;
        }
    }

    public ProfileData(string assetPath, ProfileTypes type, bool isDefault = false)
    {
        this.AssetPath = assetPath;
        IsDefault = isDefault;
        Type = type;
    }

    public void Apply()
    {
        Importer.userData = JsonUtility.ToJson(this);
        Importer.SaveAndReimport();
    }
}