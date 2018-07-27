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
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class ImportProfiles
{
    private static Dictionary<ProfileTypes, string> profileExtensions = new Dictionary<ProfileTypes, string>()
    {
        {ProfileTypes.Model, ".obj"},
        {ProfileTypes.Texture, ".jpg"}
    };

    private static Dictionary<string, ProfileTypes> reverseProfileExtensions = new Dictionary<string, ProfileTypes>()
    {
        {".obj", ProfileTypes.Model},
        {".jpg", ProfileTypes.Texture}
    };

    private static Dictionary<ProfileTypes, Func<string, Object>> assetCreationfunctions =
        new Dictionary<ProfileTypes, Func<string, Object>>()
        {
            {ProfileTypes.Model, AssetCreationFunctions.CreateModel},
            {ProfileTypes.Texture, AssetCreationFunctions.CreateTexture}
        };

    private static List<ProfileData> profiles = new List<ProfileData>();

    static ImportProfiles()
    {
        new ImporterExtension(Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ModelImporterEditor"));

        var assets = AssetDatabase.FindAssets("", new[] {"Assets/ImportProfiles/Editor/Profiles"});
        assets = assets.Distinct().ToArray();
        foreach (var asset in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var extension = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                if (reverseProfileExtensions.ContainsKey(extension))
                {
                    AddProfile(AssetImporter.GetAtPath(path));
                }
            }
        }
    }

    public static string GetExtension(ProfileTypes type)
    {
        string val;
        profileExtensions.TryGetValue(type, out val);
        return val;
    }
    
    public static void AddProfile(AssetImporter profile)
    {
        var data = JsonUtility.FromJson<ProfileData>(profile.userData);
        if (data != null)
            profiles.Add(data);
    }

    public static ProfileData CreateProfile(ProfileTypes type, string name)
    {
        string path = string.Format("Assets/ImportProfiles/Editor/Profiles/{0}/{1}{2}", type.ToString() + 's',
            name, profileExtensions[type]);
        if (!AssetDatabase.IsValidFolder(Path.GetDirectoryName(path)?.Replace('\\', '/')))
        {
            AssetDatabase.CreateFolder("Assets/ImportProfiles/Editor/Profiles", type.ToString() + 's');
        }

        if (AssetDatabase.LoadAssetAtPath<Object>(path) != null)
            path = AssetDatabase.GenerateUniqueAssetPath(path);

        Func<string, Object> value;
        if (!assetCreationfunctions.TryGetValue(type, out value))
            return null;

        value.Invoke(path);
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();

        var importer = AssetImporter.GetAtPath(path);
        if (importer != null)
        {
            var profile = new ProfileData(path, type, false);
            AddProfile(importer);
            return profile;
        }

        return null;
    }

    public static void RemoveProfile(ProfileData profile)
    {
        profiles.Remove(profile);
        AssetDatabase.DeleteAsset(profile.AssetPath);
    }

    public static void UpdateDefault(ProfileData data)
    {
        var profiles = GetProfiles(data.Type);
        foreach (ProfileData profileData in profiles)
        {
            if (profileData == data)
                data.Apply();
            else
            {
                if (profileData.IsDefault)
                {
                    profileData.IsDefault = false;
                    profileData.Apply();
                }
            }
        }
    }

    public static void RemoveNulls()
    {
        for (var i = profiles.Count - 1; i >= 0; i--)
        {
            if (profiles[i].Importer == null)
                profiles.RemoveAt(i);
        }
    }

    public static IEnumerable<ProfileData> GetProfiles()
    {
        RemoveNulls();
        return profiles;
    }

    public static IEnumerable<ProfileData> GetProfiles(Type type)
    {
        RemoveNulls();
        return profiles.Where(x => x.Importer.GetType() == type);
    }

    public static IEnumerable<ProfileData> GetProfiles(ProfileTypes type)
    {
        RemoveNulls();
        return profiles.Where(x => x.Type == type);
    }
}