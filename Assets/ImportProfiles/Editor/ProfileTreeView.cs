using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ProfileTreeView : TreeView
{
    public Action<ProfileData> OnSelectionChanged;
    public Action<ProfileData> OnRemoveProfile;
    
    private IEnumerable<ProfileData> profiles;

    private Dictionary<int, ProfileData> items = new Dictionary<int, ProfileData>();
    
    public ProfileTreeView(TreeViewState state) : base(state)
    {
    }

    public ProfileTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
    {
    }

    public void SetProfiles(IEnumerable<ProfileData> profiles)
    {
        this.profiles = profiles;
        if(this.profiles != null)
            Reload();
    }

    public void SelectItem(ProfileData data)
    {
        foreach (var keyValuePair in items)
        {
            if (keyValuePair.Value.AssetPath == data.AssetPath)
            {
                SetSelection(new []{keyValuePair.Key});
            }
        }
    }

    protected override bool CanRename(TreeViewItem item)
    {
        return GetItemById(item.id) != null;
    }

    protected override void RenameEnded(RenameEndedArgs args)
    {
        if (args.acceptedRename)
        {
            var profile = GetItemById(args.itemID);
            profile.Name = args.newName;
            Reload();
        }
        base.RenameEnded(args);
    }

    protected override void SelectionChanged(IList<int> selectedIds)
    {
        base.SelectionChanged(selectedIds);
        if (selectedIds.Any())
        {
            var profileData = GetItemById(selectedIds.First());
            OnSelectionChanged?.Invoke(profileData);
        }
    }

    protected override TreeViewItem BuildRoot()
    {
        items.Clear();
        var root = new TreeViewItem(0, -1, "Profiles");
        var id = 0;
        var profilesItem = new TreeViewItem(++id, 0, "Profiles");
        root.AddChild(profilesItem);

        var ordered = profiles.GroupBy(x => x.Type);
        foreach (var profileData in ordered)
        {
            var parent = new TreeViewItem(++id, 1, profileData.Key.ToString() + 's');
            profilesItem.AddChild(parent);
            foreach (var data in profileData)
            {
                parent.AddChild(new TreeViewItem(++id, 2, Path.GetFileNameWithoutExtension(data.AssetPath)));
                items.Add(id, data);
            }
        }

        return root;
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        base.RowGUI(args);

        var profile = GetItemById(args.item.id);
        if (profile == null || args.isRenaming)
            return;
        
        Rect buttonRect = new Rect(args.rowRect.x + args.rowRect.width - EditorGUIUtility.singleLineHeight, args.rowRect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
        Rect isDefaultRect = new Rect(args.rowRect.x + args.rowRect.width - EditorGUIUtility.singleLineHeight * 2, args.rowRect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(buttonRect, "x"))
        {
            OnRemoveProfile?.Invoke(GetItemById(args.item.id));
        }
        EditorGUI.BeginChangeCheck();

        profile.IsDefault = GUI.Toggle(isDefaultRect, profile.IsDefault, "");
        
        if (EditorGUI.EndChangeCheck())
        {
            ImportProfiles.UpdateDefault(profile);
        }
    }

    public ProfileData GetItemById(int id)
    {
        ProfileData val;
        items.TryGetValue(id, out val);
        return val;
    }
}