using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This tool looks for missing references in Assets folder
/// </summary>
public class AssetChecker : EditorWindow
{
    private bool isButtonClicked;
    private List<string> missingReferencePaths = new List<string>();
    private Vector2 scrollPosition;

    [MenuItem("Window/AssetsChecker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AssetChecker));
    }

    void OnGUI()
    {
        Vector2 windowSize = position.size;
        isButtonClicked = GUILayout.Button("Look for missing references", GUILayout.Width(windowSize.x), GUILayout.MaxWidth(200));
        GUILayout.Label($"Missing references are found: {missingReferencePaths.Count}", EditorStyles.largeLabel);

        if (isButtonClicked)
        {
            missingReferencePaths = LookForAssetPaths();
        }
        if (missingReferencePaths != null && missingReferencePaths.Count > 0)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < missingReferencePaths.Count; i++)
            {
                EditorGUILayout.LabelField($"{missingReferencePaths[i]}");
            }
            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Returnes array of missing reference paths</returns>
    private List<string> LookForAssetPaths()
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths();
        List<string> result = new List<string>(assetPaths.Length);

        for (int i = 0; i < assetPaths.Length; i++)
        {
            string path = assetPaths[i];
            if (path.StartsWith("Assets"))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                    result.Add(path);
            }
        }

        return result;
    }
}
