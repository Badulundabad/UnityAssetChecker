using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This tool looks for missing references in Assets folder
/// </summary>
public class AssetChecker : EditorWindow
{
    private int lastRefAmount;
    private bool isUpdateLoopActive;
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
        DrawButtons();
        GUILayout.Label($"Missing references are found: {missingReferencePaths.Count}", EditorStyles.largeLabel);

        if (isUpdateLoopActive || isButtonClicked)
        {
            UpdateMissingRefsList();
            if (lastRefAmount != missingReferencePaths.Count)
            {
                Debug.LogError($"Asset checker has found {missingReferencePaths.Count} missing references");
                lastRefAmount = missingReferencePaths.Count;
            }
        }
        if (missingReferencePaths != null && missingReferencePaths.Count > 0)
        {
            DrawRefList();
        }
    }

    private void DrawButtons()
    {
        Vector2 windowSize = position.size;
        GUILayout.BeginHorizontal();
        isUpdateLoopActive = GUILayout.Toggle(isUpdateLoopActive, "Update loop", GUILayout.Width(88), GUILayout.Height(20));
        GUILayout.Space(10);
        if (!isUpdateLoopActive)
        {
            isButtonClicked = GUILayout.Button("Look for missing references", GUILayout.Width(windowSize.x), GUILayout.MaxWidth(200));
        }
        GUILayout.EndHorizontal();
    }

    private void DrawRefList()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < missingReferencePaths.Count; i++)
        {
            EditorGUILayout.LabelField($"{missingReferencePaths[i]}");
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Updates missingReferencePaths field
    /// </summary>
    private void UpdateMissingRefsList()
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths();
        missingReferencePaths = new List<string>(assetPaths.Length);

        for (int i = 0; i < assetPaths.Length; i++)
        {
            string path = assetPaths[i];
            if (path.StartsWith("Assets"))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                    CollectMissingRefsFromObject(missingReferencePaths, path, obj);
            }
        }
    }

    private void CollectMissingRefsFromObject(List<string> list, string path, GameObject obj)
    {
        Component[] components = obj.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            Component component = components[i];
            if (!component)
            {
                list.Add($"{path} is missing component #{i}");
            }
            else
            {
                var serializedObject = new SerializedObject(component);
                var serializedProperty = serializedObject.GetIterator();
                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference
                        && serializedProperty.objectReferenceValue == null
                        && serializedProperty.objectReferenceInstanceIDValue != 0)
                    {
                        list.Add($"{path}/{component.GetType().Name} is missing reference");
                    }
                }
            }
        }
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            GameObject child = obj.transform.GetChild(i).gameObject;
            CollectMissingRefsFromObject(list, $"{path}/{child.name}", child);
        }
    }
}
