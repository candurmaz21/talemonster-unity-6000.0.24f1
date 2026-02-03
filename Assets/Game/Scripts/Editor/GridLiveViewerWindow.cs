#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Game.GridModule.Controller;
using Game.GridModule.Model;
using VContainer;
using VContainer.Unity;
using Game.HeroModule.Model;
using Game.HeroModule.Service;

public sealed class GridLiveViewerWindow : EditorWindow
{
    private IGridProvider gridProvider;
    private IGridTransformProvider gridTransformProvider;
    private HeroViewRegistery viewRegistery;
    private Vector2 scroll;

    [MenuItem("Tools/Grid/Live Grid Viewer")]
    static void Open() => GetWindow<GridLiveViewerWindow>("Grid Viewer");

    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Game is not running", MessageType.Info);
            return;
        }

        if (gridProvider == null || viewRegistery == null || gridTransformProvider == null)
        {
            TryResolveDependencies();
        }

        if (gridProvider == null)
        {
            EditorGUILayout.HelpBox("GridController not found", MessageType.Warning);
            return;
        }

        DrawHeader();
        DrawGrid();
    }

    void TryResolveDependencies()
    {
        foreach (var scope in Object.FindObjectsByType<LifetimeScope>(FindObjectsSortMode.None))
        {
            if (scope.Container == null) continue;

            if (gridProvider == null)
            {
                scope.Container.TryResolve(out gridProvider);
            }

            if (gridTransformProvider == null)
            {
                scope.Container.TryResolve(out gridTransformProvider);
            }

            if (viewRegistery == null)
            {
                scope.Container.TryResolve(out viewRegistery);
            }

        }
    }

    void DrawHeader()
    {
        EditorGUILayout.LabelField("Grid Size: " + gridProvider.GridWidth + " x " + gridProvider.GridHeight);
        EditorGUILayout.LabelField("World Size: " + gridTransformProvider.GridWorldWidth + " x " + gridTransformProvider.GridWorldHeight);

        GUILayout.Space(10);
    }

    void DrawGrid()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int y = gridProvider.GridHeight - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < gridProvider.GridWidth; x++)
            {
                Cell cell = gridProvider.GetCell(x, y);

                GUI.backgroundColor = cell.isEmpty ? Color.gray : GetColor(cell.hero.Type);

                if (GUILayout.Button(
                    cell.isEmpty ? "Empty" : cell.hero.Type.ToString() + "\n" + cell.hero.Level.ToString(),
                    GUILayout.Width(70),
                    GUILayout.Height(40)))
                {
                    var view = viewRegistery?.GetView(cell);

                    if (view != null)
                    {
                        Selection.activeObject = view.gameObject;
                        EditorGUIUtility.PingObject(view.gameObject);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndScrollView();
    }

    Color GetColor(HeroType type)
    {
        return type switch
        {
            HeroType.King => Color.yellow,
            HeroType.Knight => Color.cyan,
            HeroType.Goblin => Color.green,
            HeroType.Frosty => Color.blue,
            _ => Color.white
        };
    }

}
#endif