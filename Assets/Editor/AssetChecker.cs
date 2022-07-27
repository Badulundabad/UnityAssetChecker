using UnityEditor;
using UnityEngine;

/// <summary>
/// This tool looks for missing references in Assets folder
/// </summary>
public class AssetChecker : EditorWindow
{
    private bool isButtonClicked;

    [MenuItem("Window/AssetsChecker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AssetChecker));
    }

    void OnGUI()
    {
        GUILayout.Label("Missing references", EditorStyles.boldLabel);
        isButtonClicked = GUILayout.Button("Look for missing references");
    }
}
