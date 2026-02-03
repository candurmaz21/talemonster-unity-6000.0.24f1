#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Game.LevelModule.Config;
using System.Collections.Generic;
using Game.HeroModule.Model;

public sealed class LevelEditorWindow : EditorWindow
{
    int width = 6;
    int height = 6;

    LevelLayoutData existingLayout;
    LevelLayoutData lastLoadedLayout;

    Dictionary<Vector2Int, CellData> cells = new();

    [MenuItem("Tools/Level Editor")]
    static void Open() => GetWindow<LevelEditorWindow>();

    void OnGUI()
    {
        DrawLayoutSlot();
        DrawClearButton();
        DrawSizeFields();
        DrawGrid();

        GUILayout.Space(10);

        if (GUILayout.Button("Save"))
            Save();
    }

    void DrawLayoutSlot()
    {
        var selected = (LevelLayoutData)EditorGUILayout.ObjectField(
            "Level Layout",
            existingLayout,
            typeof(LevelLayoutData),
            false
        );

        if (selected != existingLayout)
        {
            existingLayout = selected;
            LoadFromLayout(existingLayout);
        }
    }

    void DrawClearButton()
    {
        using (new EditorGUI.DisabledScope(existingLayout == null))
        {
            if (GUILayout.Button("Clear Layout"))
            {
                existingLayout = null;
                lastLoadedLayout = null;
                cells.Clear();
            }
        }
    }

    void DrawSizeFields()
    {
        using (new EditorGUI.DisabledScope(existingLayout != null))
        {
            EditorGUILayout.BeginHorizontal();
            width = EditorGUILayout.IntField("Width", width);
            height = EditorGUILayout.IntField("Height", height);
            EditorGUILayout.EndHorizontal();
        }
    }

    void DrawGrid()
    {
        if (width <= 0 || height <= 0)
            return;

        for (int y = height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < width; x++)
                DrawCell(x, y);

            EditorGUILayout.EndHorizontal();
        }
    }

    void DrawCell(int x, int y)
    {
        var key = new Vector2Int(x, y);

        if (!cells.TryGetValue(key, out var data))
            data = new CellData { x = x, y = y };

        EditorGUILayout.BeginVertical(GUILayout.Width(80));
        data.heroType = (HeroType)EditorGUILayout.EnumPopup(data.heroType);
        data.level = EditorGUILayout.IntField(data.level);
        EditorGUILayout.EndVertical();

        cells[key] = data;
    }

    void LoadFromLayout(LevelLayoutData layout)
    {
        cells.Clear();

        if (layout == null)
            return;

        width = layout.width;
        height = layout.height;

        foreach (var cell in layout.cells)
            cells[new Vector2Int(cell.x, cell.y)] = cell;

        lastLoadedLayout = layout;
    }

    void Save()
    {
        LevelLayoutData asset;

        if (existingLayout != null)
        {
            asset = existingLayout;
        }
        else
        {
            asset = ScriptableObject.CreateInstance<LevelLayoutData>();

            var basePath = "Assets/Game/Scriptables/Generated/Level_" + width + "x" + height + ".asset";
            var path = AssetDatabase.GenerateUniqueAssetPath(basePath);

            AssetDatabase.CreateAsset(asset, path);
        }

        asset.width = width;
        asset.height = height;
        asset.cells = new List<CellData>(cells.Values).ToArray();

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
    }
}
#endif