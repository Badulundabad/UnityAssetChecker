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
    private List<KeyValuePair<string, string>> missingReferencePaths;
    private Vector2 scrollPosition;

    [MenuItem("Window/Assets Checker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AssetChecker));
    }

    void OnGUI()
    {
        DrawButtons();
        GUILayout.Label($"Missing references are found: {missingReferencePaths?.Count}", EditorStyles.largeLabel);

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
            isButtonClicked = GUILayout.Button("Find missing references", GUILayout.Width(windowSize.x), GUILayout.MaxWidth(200));
        }
        GUILayout.EndHorizontal();
    }

    private void DrawRefList()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        foreach (var kvp in missingReferencePaths)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{kvp.Key}", EditorStyles.objectField);
            EditorGUILayout.LabelField($"{kvp.Value}", EditorStyles.objectField, GUILayout.MaxWidth(160));
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Updates missingReferencePaths field
    /// </summary>
    private void UpdateMissingRefsList()
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths();
        missingReferencePaths = new List<KeyValuePair<string, string>>(assetPaths.Length);

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

    private void CollectMissingRefsFromObject(List<KeyValuePair<string, string>> list, string path, GameObject obj)
    {
        Component[] components = obj.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            Component component = components[i];
            if (!component)
            {
                list.Add(new KeyValuePair<string, string>(path, $"missing component #{i}"));
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
                        list.Add(new KeyValuePair<string, string>(path, $"{component.GetType().Name}"));
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
