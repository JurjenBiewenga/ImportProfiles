using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[CustomEditor(typeof(ModelImporter))]
public class ImportProfileImporterEditor : DecoratorEditor
{
    private int selected;
    private string[] names;
    private SerializedProperty userDataProperty;
    private List<string> assetImportersPaths;

    public AssetImporter importer
    {
        get
        {
            return target as AssetImporter;
        }
    }

    public ImportProfileImporterEditor() : base("UnityEditor.ModelImporterEditor")
    {
    }

    private void OnEnable()
    {
        assetImportersPaths = ImportProfiles.Editor.ImportProfiles.GetImporters(target.GetType()).Select(x=> x.assetPath).ToList();
        assetImportersPaths.Add("Custom");

        if (assetImportersPaths != null)
        {
            var list = assetImportersPaths.Select(Path.GetFileNameWithoutExtension).ToList();
            names = list.ToArray();
        }

        userDataProperty = serializedObject.FindProperty("m_UserData");
        if (!string.IsNullOrWhiteSpace(userDataProperty.stringValue))
        {
            selected = assetImportersPaths.IndexOf(userDataProperty.stringValue);
        }
    }

    public override void OnInspectorGUI()
    {
        if (this.importer.assetPath.Contains("ImportProfiles"))
        {
            base.OnInspectorGUI();
            return;
        }
        
        if (names != null)
        {
            EditorGUI.BeginChangeCheck();
            selected = EditorGUILayout.Popup("Selected Profile", selected, names);
            if (EditorGUI.EndChangeCheck())
                userDataProperty.stringValue = assetImportersPaths[selected];

            GUI.enabled = selected == names.Length - 1;
        }

        base.OnInspectorGUI();
        
        GUI.enabled = true;
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        RevertButton();
        if (ApplyButton())
        {
            ReloadPreviewInstances();
            ResetValues();
            serializedObject.Update();
            Repaint();
        }
        GUILayout.EndHorizontal();
    }
}    
