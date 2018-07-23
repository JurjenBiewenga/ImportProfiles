using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

public class ImportProfileImporter : AssetPostprocessor 
{
    void OnPreprocessModel()
    {
        if (this.assetPath.Contains("ImportProfiles"))
            return;
        
        var importer = AssetImporter.GetAtPath("Assets/ImportProfiles/Editor/Profiles/Models/test.obj");
        if(importer == null || assetImporter.userData == "Custom")
            return;
        
        SerializedObject so = new SerializedObject(importer);

        SerializedObject targetSO = new SerializedObject(assetImporter);
        
        var property = so.GetIterator();
        property.Next(true);
        do
        {
            if(property.name == "m_UserData")
                continue;
            
            var targetProperty = targetSO.FindProperty(property.propertyPath);
            targetProperty?.SetValue(property.GetValue());
        } while (property.Next(false));

        targetSO.ApplyModifiedPropertiesWithoutUndo();
    }
}
