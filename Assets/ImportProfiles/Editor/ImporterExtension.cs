using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

public class ImporterExtension
{
    private const BindingFlags Flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public |
                                              System.Reflection.BindingFlags.FlattenHierarchy;

    private Type[] types;

    private MemberInfo apply;
    
    public ImporterExtension(params Type[] types)
    {
        this.types = types;
        var harmony = HarmonyInstance.Create("com.lunosis.importprofiles");
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
        AssetImporterEditor editor = __instance as AssetImporterEditor;

        if (editor == null)
            return;

        string path = AssetDatabase.GetAssetPath(editor.target);

        if (path.Contains("ImportProfiles"))
            return;

        var assetImportersPaths =
            ImportProfiles.GetImporters(editor.target.GetType()).Select(x => x.assetPath).ToList();

        string[] names = null;
        assetImportersPaths.Add("Custom");
        var list = assetImportersPaths.Select(Path.GetFileNameWithoutExtension).ToList();
        names = list.ToArray();

        var userDataProperty = editor.serializedObject.FindProperty("m_UserData");
        GUILayout.BeginHorizontal();
        userDataProperty.stringValue =
            assetImportersPaths[
                EditorGUILayout.Popup("Selected Profile", assetImportersPaths.IndexOf(userDataProperty.stringValue),
                    names)];
//        if (GUILayout.Button("Apply", GUILayout.MaxWidth(80)))
//        {
//            typeof(AssetImporterEditor).GetMethod("ApplyAndImport", Flags)?.Invoke(editor, null);
//        }

        GUILayout.EndHorizontal();

        if (userDataProperty.stringValue != "Custom")
            GUI.enabled = false;
    }

    public static void ApplyRevertPrefix()
    {
        GUI.enabled = true;
    }
}
