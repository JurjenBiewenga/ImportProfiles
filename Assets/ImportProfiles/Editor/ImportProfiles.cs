using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Harmony;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[InitializeOnLoad]
public static class ImportProfiles
{
    public static IEnumerable<KeyValuePair<ProfileTypes, string>> ProfileExtensions
    {
        get { return profileExtensions; }
    }

    private static Dictionary<ProfileTypes, string> profileExtensions = new Dictionary<ProfileTypes, string>()
    {
        {ProfileTypes.Model, ".obj"},
        {ProfileTypes.Texture, ".png"}
    };

    private static Dictionary<string, ProfileTypes> reverseProfileExtensions = new Dictionary<string, ProfileTypes>()
    {
        {".obj", ProfileTypes.Model},
        {".png", ProfileTypes.Texture}
    };

    private static Dictionary<Type, List<AssetImporter>> profiles = new Dictionary<Type, List<AssetImporter>>();

    private static Dictionary<ProfileTypes, List<AssetImporter>> typeProfiles =
        new Dictionary<ProfileTypes, List<AssetImporter>>();

    private static List<ProfileData> profileData = new List<ProfileData>();
    
    static ImportProfiles()
    {
        new ImporterExtension(Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ModelImporterEditor"));

        var assets = AssetDatabase.FindAssets("", new[] {"Assets/ImportProfiles/Editor/Profiles"});
        assets = assets.Distinct().ToArray();
        foreach (var asset in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var extension = Path.GetExtension(path);
            if (reverseProfileExtensions.ContainsKey(extension))
            {
                var type = reverseProfileExtensions[extension];
                AddProfile(type, AssetImporter.GetAtPath(path), false);
            }
        }
    }

    public static void AddProfile(ProfileTypes type, AssetImporter profile, bool isDefault = false)
    {
        if (!typeProfiles.ContainsKey(type))
            typeProfiles.Add(type, new List<AssetImporter>());

        typeProfiles[type].Add(profile);

        var importerType = profile.GetType();
        if (!profiles.ContainsKey(importerType))
            profiles.Add(importerType, new List<AssetImporter>());

        profiles[importerType].Add(profile);
        profileData.Add(JsonUtility.FromJson<ProfileData>(profile.userData));
    }

    public static void CreateProfile(ProfileTypes type, string name)
    {
        var asset = new TextAsset();
        string path = string.Format("Assets/ImportProfiles/Editor/Profiles/{0}/{1}{2}", type.ToString() + 's',
            name, profileExtensions[type]);
        
        
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();

        var importer = AssetImporter.GetAtPath(path);
        new ProfileData(path, type, false).Apply();
        AddProfile(type, importer, false);
    }

    public static IEnumerable<AssetImporter> GetProfiles(Type type)
    {
        return profileData.Where(x=>x.Importer.GetType() == type).Select(x=>x.Importer);
    }
    
    public static IEnumerable<AssetImporter> GetProfiles(ProfileTypes type)
    {
        return profileData.Where(x=>x.Type == type).Select(x=>x.Importer);
    }
}