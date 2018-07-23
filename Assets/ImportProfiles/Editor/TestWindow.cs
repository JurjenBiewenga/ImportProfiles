using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

public class TestWindow : EditorWindow
{

    private AssetImporterEditor editor;
    private AssetImporter importer;
    
    [MenuItem("Window/test")]
    public static void ShowWindow()
    {
        GetWindow<TestWindow>().Show();
    }

    void OnEnable()
    {
        TestData data = new TestData();

//        AssetDatabase.CreateAsset(new TextAsset(),  );
        
        var type = Type.GetType("UnityEditor.ModelImporterEditor,UnityEditor");
        importer = AssetImporter.GetAtPath("Assets/Editor/Resources/test2.obj");
//        var method = type.GetMethod("InternalSetTargets",
//            BindingFlags.Default | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic);
//        editor = (AssetImporterEditor) Activator.CreateInstance(type);
        editor = (AssetImporterEditor) Editor.CreateEditor(importer);
        Debug.Log("Type" + type);
        Debug.Log("Importer" + importer);
//       editor = (AssetImporterEditor)CreateInstance(type);
//        method.Invoke(editor, BindingFlags.Default | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, new [] {new Object[]{data}},  CultureInfo.CurrentCulture);
        Debug.Log("Editor" + editor);
        editor.OnEnable();
    }

    private void OnGUI()
    {
        editor.OnInspectorGUI();
    }
}