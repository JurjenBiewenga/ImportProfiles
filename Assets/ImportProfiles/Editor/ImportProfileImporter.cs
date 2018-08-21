using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ImportProfileImporter : AssetPostprocessor
{
    private static HashSet<string> ignoredProperties = new HashSet<string>()
    {
        "m_UserData"
    };

    private void ApplyProfile()
    {
        if (assetPath.Contains("ImportProfiles"))
            return;

        if (string.IsNullOrWhiteSpace(assetImporter.userData))
        {
            ProfileData defaultProfile = ImportProfiles.GetProfiles(assetImporter.GetType()).FirstOrDefault(x => Regex.IsMatch(assetImporter.assetPath,WildCardToRegular( x.WildcardQuery)));
            if (defaultProfile != null)
                assetImporter.userData = defaultProfile.AssetPath;
            else
                return;
        }
        
        if (assetImporter.userData == "Custom")
            return;

        AssetImporter importer = AssetImporter.GetAtPath(assetImporter.userData);
        
        if(importer == null)
            return;
                 
        var so = new SerializedObject(importer);

        var targetSO = new SerializedObject(assetImporter);

        SerializedProperty property = so.GetIterator();
        property.Next(true);
        do
        {
            if (ignoredProperties.Contains(property.name))
                continue;

            targetSO.CopyFromSerializedProperty(property);
        } while (property.Next(false));

        targetSO.ApplyModifiedPropertiesWithoutUndo();
    }
    
    private void OnPreprocessModel()
    {
        ApplyProfile();
    }

    private void OnPreprocessTexture()
    {
        ApplyProfile();
    }
    
    private static string WildCardToRegular(string value) {
        return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$"; 
    }
}