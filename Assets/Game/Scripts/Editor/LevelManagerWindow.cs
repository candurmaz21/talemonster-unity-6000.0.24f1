#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class LevelManagerWindow : EditorWindow
{
    private const string LEVEL_INDEX_KEY = "CurrentLevelIndex";
    private int targetLevel = 0;

    [MenuItem("Tools/Level Manager")]
    public static void ShowWindow()
    {
        GetWindow<LevelManagerWindow>("Level Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Settings", EditorStyles.boldLabel);

        int currentLevel = PlayerPrefs.GetInt(LEVEL_INDEX_KEY, 0);

        string infoText = "Current level: " + currentLevel + "\n(Display level: " + (currentLevel + 1) + ")";
        EditorGUILayout.HelpBox(infoText, MessageType.Info);

        GUILayout.Space(10);

        targetLevel = EditorGUILayout.IntField("Set level index to ", targetLevel);

        if (GUILayout.Button("Update level index"))
        {
            SetLevel(targetLevel);
        }

        GUILayout.Space(5);

        GUI.color = Color.red;
        if (GUILayout.Button("Reset to level 0 (delete save)"))
        {
            if (EditorUtility.DisplayDialog("Reset progress?", "You sure to reset level 0", "Yes", "No"))
            {
                SetLevel(0);
                targetLevel = 0;
            }
        }
        GUI.color = Color.white;
    }

    private void SetLevel(int index)
    {
        PlayerPrefs.SetInt(LEVEL_INDEX_KEY, index);
        PlayerPrefs.Save();

        Debug.Log("<color=green>Level index updated to: </color>" + index);
    }
}
#endif