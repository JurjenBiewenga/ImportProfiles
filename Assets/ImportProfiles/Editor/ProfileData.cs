using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ProfileData
{
    public string AssetPath;
    public ProfileTypes Type;
    public string WildcardQuery;

    private AssetImporter importer;

    private string name;
    
    public AssetImporter Importer
    {
        get
        {
            if (importer == null)
                importer = AssetImporter.GetAtPath(AssetPath);
            return importer;
        }
    }

    public string Name
    {
        get
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Path.GetFileNameWithoutExtension(AssetPath);
            }

            return name;
        }

        set
        {
            if(Name == value)
                return;
            
            AssetDatabase.RenameAsset(AssetPath, value);
            string extension = Path.GetExtension(AssetPath);
            AssetPath = AssetPath.Replace(name + extension, value + extension);
            name = value;
            Apply();
        }
    }

    public ProfileData(string assetPath, ProfileTypes type)
    {
        AssetPath = assetPath;
        Type = type;
        if (Importer != null)
            Apply();
    }

    public void Apply()
    {
        Importer.userData = JsonUtility.ToJson(this);
        AssetDatabase.ImportAsset(AssetPath);
    }
}