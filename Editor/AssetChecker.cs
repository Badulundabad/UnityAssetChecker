using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetChecker
{
    /// <summary>
    /// This tool looks for missing references in Assets folder
    /// </summary>
    public class AssetChecker : EditorWindow
    {
        private bool isCheckInProgress;
        private List<Object> foundObjects;
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
                    RunAssetCheck();
                }
            }
        }

        private void ShowResult()
        {
            resultWindow = AssetCheckerResult.ShowResult(foundObjects);
        }

        private void RunAssetCheck()
        {
            isCheckInProgress = true;
            foundObjects = new List<Object>();

            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < paths.Length; i++)
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
                if (obj)
                {
                    CheckGameObject(obj);
                }
            }
            isCheckInProgress = false;
            OnCheckComplete?.Invoke();
        }

        private void CheckGameObject(GameObject obj)
        {
            var components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (!component)
                    foundObjects.Add(obj);
                else
                    CheckComponentProperties(component);
            }
        }

        private void CheckComponentProperties(Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference
                    && serializedProperty.objectReferenceValue == null
                    && serializedProperty.objectReferenceInstanceIDValue != 0)
                {
                    foundObjects.Add(component);
                }
            }
        }
    }
}