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
                    RunCheck();
                }
            }
        }

        private void ShowResult()
        {
            resultWindow = AssetCheckerResult.ShowResult(foundObjects);
        }

        private void RunCheck()
        {
            isCheckInProgress = true;
            foundObjects = new List<Object>();

            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < paths.Length; i++)
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
                if (obj)
                {
                    CheckObject(obj);
                }
            }
            isCheckInProgress = false;
            OnCheckComplete?.Invoke();
        }

        private void CheckObject(GameObject obj)
        {
            var hierarchyObjects = EditorUtility.CollectDeepHierarchy(new GameObject[] { obj });
            for (int i = 0; i < hierarchyObjects.Length; i++)
            {
                var item = hierarchyObjects[i];
                if (!item)
                    foundObjects.Add(obj);
                else
                    CheckObjectComponents(item);
            }
        }

        private void CheckObjectComponents(Object obj)
        {
            var serializedObject = new SerializedObject(obj);
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference
                    && serializedProperty.objectReferenceValue == null
                    && serializedProperty.objectReferenceInstanceIDValue != 0)
                {
                    foundObjects.Add(obj);
                }
            }
        }
    }
}