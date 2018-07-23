using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace ImportProfiles.Editor
{
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
        private static Dictionary<ProfileTypes, List<AssetImporter>> typeProfiles = new Dictionary<ProfileTypes, List<AssetImporter>>();
        
        static ImportProfiles()
        {
            var assets = AssetDatabase.FindAssets("", new[] {"Assets/ImportProfiles/Editor/Profiles"});
            assets = assets.Distinct().ToArray();
            foreach (var asset in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(asset);
                var extension = Path.GetExtension(path);
                if (reverseProfileExtensions.ContainsKey(extension))
                {
                    var type = reverseProfileExtensions[extension];
                    AddProfile(type, AssetImporter.GetAtPath(path));
                }
            }
        }

        public static void AddProfile(ProfileTypes type, AssetImporter profile)
        {
            if (!typeProfiles.ContainsKey(type))
                typeProfiles.Add(type, new List<AssetImporter>());
            
            typeProfiles[type].Add(profile);

            var importerType = profile.GetType();
            if(!profiles.ContainsKey(importerType))
                profiles.Add(importerType, new List<AssetImporter>());
            
            profiles[importerType].Add(profile);
        }

        public static void CreateProfile(ProfileTypes type, string name)
        {
            var asset =new TextAsset();
            string path = string.Format("ImportProfiles/Editor/Profiles/{0}/{1}.{2}", type.ToString(),
               name,  profileExtensions[type]);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.ImportAsset(path);
            AssetDatabase.SaveAssets();

            var importer = AssetImporter.GetAtPath(path);
            AddProfile(type, importer);
        }
        
        public static List<AssetImporter> GetImporters(Type t)
        {
            if (profiles.ContainsKey(t))
            {
                return profiles[t];
            }

            return null;
        }
        
        public static List<AssetImporter> GetImporters(ProfileTypes t)
        {
            if (typeProfiles.ContainsKey(t))
            {
                return typeProfiles[t];
            }

            return null;
        }
    }

    public enum ProfileTypes
    {
        Model,
        Texture
    }
}