using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;


namespace Assets.Editor
{
    /// <summary>
    /// This tool looks for missing references in Assets folder
    /// </summary>
    public class AssetChecker : EditorWindow
    {
        private int currentProgress;
        private bool isCheckInProgress;
        private List<KeyValuePair<string, string>> missingReferences;
        private EditorWindow resultWindow;
        private event Action OnCheckComplete;

        [MenuItem("Window/Asset Checker")]
        public static void ShowWindow()
        {
            var assetChecker = EditorWindow.GetWindow<AssetChecker>("Asset Checker");
            assetChecker.minSize = new Vector2(161, 40);
            assetChecker.maxSize = new Vector2(161, 0);
        }

        private void OnEnable()
        {
            isCheckInProgress = false;
            OnCheckComplete += ShowResult;
        }

        private void OnDestroy()
        {
            OnCheckComplete -= ShowResult;
        }

        void OnGUI()
        {
            if (!isCheckInProgress)
            {
                if (GUILayout.Button("Check assets", GUILayout.Width(position.size.x), GUILayout.MaxWidth(155)))
                {
                    if (resultWindow != null)
                        resultWindow.Close();
                    EditorCoroutineUtility.StartCoroutine(Check(), this);
                }
            }
            if (isCheckInProgress)
            {
                GUILayout.Label("Checking...", EditorStyles.largeLabel);
                GUILayout.Label($"Already found: {currentProgress}", EditorStyles.largeLabel);
            }
        }

        private void ShowResult()
        {
            resultWindow = AssetCheckerResult.ShowResult(missingReferences);
        }

        private IEnumerator Check()
        {
            isCheckInProgress = true;
            missingReferences = new List<KeyValuePair<string, string>>();
            var assetPaths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < assetPaths.Length; i++)
            {
                string path = assetPaths[i];
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                    CollectMissingRefsFromObject(missingReferences, path, obj);
                yield return null;
            }
            currentProgress = 0;
            isCheckInProgress = false;
            OnCheckComplete?.Invoke();
        }

        private void CollectMissingRefsFromObject(List<KeyValuePair<string, string>> list, string path, GameObject obj)
        {
            Component[] components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (!component)
                {
                    currentProgress++;
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
                            currentProgress++;
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
}