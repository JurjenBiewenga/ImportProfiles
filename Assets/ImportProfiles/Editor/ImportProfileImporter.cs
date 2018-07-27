using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var defaultProfile = ImportProfiles.GetProfiles(assetImporter.GetType()).FirstOrDefault(x => x.IsDefault);
            if (defaultProfile != null)
                assetImporter.userData = defaultProfile.AssetPath;
            else
                return;
        }
        
        if (assetImporter.userData == "Custom")
            return;

        var importer = AssetImporter.GetAtPath(assetImporter.userData);
        
        if(importer == null)
            return;
                 
        SerializedObject so = new SerializedObject(importer);

        SerializedObject targetSO = new SerializedObject(assetImporter);

        var property = so.GetIterator();
        property.Next(true);
        do
        {
            if (ignoredProperties.Contains(property.name))
                continue;

            SerializedProperty targetProperty = targetSO.FindProperty(property.propertyPath);
            targetProperty?.SetValue(property.GetValue());
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
}