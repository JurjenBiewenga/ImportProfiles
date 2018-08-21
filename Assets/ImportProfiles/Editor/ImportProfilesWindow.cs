using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.IMGUI.Controls;
using UnityEditor.VersionControl;
using UnityEngine;
using Object = UnityEngine.Object;

public class ImportProfilesWindow : EditorWindow
{
    private const int SideBarWidth = 150;
    private const int BorderWidth = 2;
    private const int Margin = 4;

    private ProfileTreeView treeView;

    private GUIStyle areaStyle;
    private GUIStyle dropDownButtonStyle;

    [SerializeField]
    private Editor currentEditor;

    private TreeViewState treeViewState = new TreeViewState();
    private GenericMenu createMenu = new GenericMenu();
    private ProfileData currentProfile;

    [MenuItem("Window/Import Profiles Editor")]
    public static void ShowWindow()
    {
        GetWindow<ImportProfilesWindow>().Show();
    }

    void OnEnable()
    {
        treeView = new ProfileTreeView(treeViewState);
        UpdateProfiles();
        treeView.OnSelectionChanged += OnSelectionChanged;
        treeView.OnRemoveProfile += OnRemoveProfile;
        areaStyle = new GUIStyle();
        areaStyle.margin = new RectOffset(Margin, Margin, Margin, Margin);

        foreach (var profileType in Enum.GetValues(typeof(ProfileTypes)))
        {
            createMenu.AddItem(new GUIContent(profileType.ToString()), false, () =>
            {
                var profile = ImportProfiles.CreateProfile((ProfileTypes) profileType, profileType.ToString());
                UpdateProfiles();
                treeView.SelectItem(profile);
                OnSelectionChanged(profile);
            });
        }

        if (treeViewState.selectedIDs.Any())
        {
            OnSelectionChanged(treeView.GetItemById(treeViewState.selectedIDs.First()));
        }
    }

    private void OnRemoveProfile(ProfileData profile)
    {
        if (!EditorUtility.DisplayDialog($"Delete profile {profile.Name}",
            "Are you sure you wish to delete the profile?", "Yes", "No"))
            return;
        ImportProfiles.RemoveProfile(profile);
        UpdateProfiles();
    }

    private void UpdateProfiles()
    {
        treeView.SetProfiles(ImportProfiles.GetProfiles());
    }

    private void OnSelectionChanged(ProfileData profile)
    {
        if (currentEditor != null)
            DestroyImmediate(currentEditor);

        if (profile != null)
        {
            currentProfile = profile;
            currentEditor = Editor.CreateEditor(profile.Importer);
        }
    }

    private void OnDisable()
    {
        DestroyImmediate(currentEditor);
    }

    private void OnGUI()
    {
        DrawSizebar();
        DrawCurrentEditor();
    }

    private void DrawCurrentEditor()
    {
        GUILayout.BeginArea(new Rect(SideBarWidth, 0, Screen.width - SideBarWidth, Screen.height), areaStyle);

        if (currentEditor != null && currentEditor.target != null)
        {
            currentEditor.OnInspectorGUI();
            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            
            currentProfile.WildcardQuery = EditorGUILayout.TextField("Regex",currentProfile.WildcardQuery);
            
            if(EditorGUI.EndChangeCheck())
            {
                currentProfile.Apply();
            }
            
        }
        else
            UpdateProfiles();
        GUILayout.EndArea();
    }

    private void DrawSizebar()
    {
        GUILayout.BeginArea(new Rect(0, 0, SideBarWidth, Screen.height), areaStyle);

        var treeViewRect = new Rect(0, Margin, SideBarWidth - BorderWidth,
            Screen.height - EditorGUIUtility.singleLineHeight - Margin);
        treeView.OnGUI(treeViewRect);

        // Magic number because unity is weird
        var dropDownButtonRect = new Rect(Margin, Screen.height - 46,
            SideBarWidth - Margin * 2 - BorderWidth, EditorGUIUtility.singleLineHeight);

        if (GUI.Button(dropDownButtonRect, new GUIContent("Create"), "DropDown"))
        {
            createMenu.DropDown(dropDownButtonRect);
        }


        GUI.Box(new Rect(SideBarWidth - BorderWidth, 0, BorderWidth, Screen.height), "");
        GUILayout.EndArea();
    }
}