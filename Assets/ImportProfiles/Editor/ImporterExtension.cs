using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

public class ImporterExtension
{
    private const BindingFlags Flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public |
                                              System.Reflection.BindingFlags.FlattenHierarchy;

    private MemberInfo apply;
    
    public ImporterExtension(params Type[] types)
    {
        var harmony = HarmonyInstance.Create("com.jurjenbiewenga.importprofiles");
        var inspectorPre = GetType().GetMethod(nameof(InspectorPrefix), Flags);
        var applyRevertPre = GetType().GetMethod(nameof(ApplyRevertPrefix), Flags);
        
        foreach (var type in types)
        {
            var onInspectorGUI = type.GetMethod("OnInspectorGUI", Flags);
            var applyRevertGUI = type.GetMethod("ApplyRevertGUI", Flags);

            harmony.Patch(onInspectorGUI, new HarmonyMethod(inspectorPre), null);
            harmony.Patch(applyRevertGUI, new HarmonyMethod(applyRevertPre), null);
        }

    }

    // ReSharper disable once InconsistentNaming
    public static void InspectorPrefix(object __instance)
    {
        var editor = __instance as AssetImporterEditor;

        if (editor == null)
            return;

        var importer = editor.target as AssetImporter;
        if (importer == null)
            return;
        
        if(importer.assetPath.Contains("ImportProfiles"))
            return;

        List<string> assetImportersPaths =
            ImportProfiles.GetProfiles(importer.GetType()).Select(x => x.Importer.assetPath).ToList();

        GUILayout.Label(importer.userData);

        assetImportersPaths.Add("Custom");
        List<string> list = assetImportersPaths.Select(Path.GetFileNameWithoutExtension).ToList();
        string[] names = list.ToArray();

        SerializedProperty userDataProperty = editor.serializedObject.FindProperty("m_UserData");
        GUILayout.BeginHorizontal();
        int index = assetImportersPaths.IndexOf(userDataProperty.stringValue);
        if (index == -1)
            index = 0;

        string userDataValue;

        EditorGUI.BeginChangeCheck();
        userDataValue =
            assetImportersPaths[
                EditorGUILayout.Popup("Selected Profile", index,
                    names)];
        if (EditorGUI.EndChangeCheck())
        {
            userDataProperty.stringValue = userDataValue;
        }

        GUILayout.EndHorizontal();

        if (userDataProperty.stringValue != "Custom")
            GUI.enabled = false;
    }

    public static void ApplyRevertPrefix()
    {
        GUI.enabled = true;
    }
}
