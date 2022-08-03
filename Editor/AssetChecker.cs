using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetChecker
{
    /// <summary>
    /// This tool looks for missing references in Assets folder
    /// </summary>
    public class AssetChecker : EditorWindow
    {
        private bool isCheckInProgress;
        private List<MissingObjectData> assetObjects;
        private List<MissingObjectData> sceneObjects;
        private EditorWindow resultWindow;
        public static readonly string UNKNOWN = "Unknown";

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
        }

        void OnGUI()
        {
            if (!isCheckInProgress)
            {
                if (GUILayout.Button("Check assets", GUILayout.Width(position.size.x), GUILayout.MaxWidth(155)))
                {
                    if (resultWindow != null)
                        resultWindow.Close();
                    isCheckInProgress = true;
                    assetObjects = new List<MissingObjectData>();
                    sceneObjects = new List<MissingObjectData>();
                    RunCheck();
                    isCheckInProgress = false;
                    ShowResult();
                }
            }
        }

        private void ShowResult()
        {
            resultWindow = AssetCheckerResult.ShowResult(assetObjects, sceneObjects);
        }

        private void RunCheck()
        {
            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (obj)
                {
                    System.Type type = obj.GetType();
                    if (type == typeof(GameObject))
                    {
                        CheckGameObject(path, obj as GameObject);
                    }
                    else if (type == typeof(SceneAsset))
                    {
                        CheckScene(path);
                    }
                    else if (type.BaseType == typeof(ScriptableObject))
                    {
                        CheckObjectProperties(path, obj);
                    }
                }
            }
        }

        private void CheckScene(string path)
        {
            Scene scene = EditorSceneManager.OpenScene(path);
            GameObject[] sceneRootObjects = scene.GetRootGameObjects();
            for (int j = 0; j < sceneRootObjects.Length; j++)
            {
                GameObject obj = sceneRootObjects[j];
                CheckGameObject(scene.name, path, obj);
            }
        }

        private void CheckGameObject(string assetPath, GameObject obj)
        {
            var components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (!component)
                    assetObjects.Add(new MissingObjectData(assetPath, obj.name, UNKNOWN));
                else
                    CheckObjectProperties(assetPath, component);
            }
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                CheckGameObject(assetPath, child);
            }
        }

        private void CheckGameObject(string sceneName, string scenePath, GameObject obj)
        {
            var components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (!component)
                {
                    var id = GlobalObjectId.GetGlobalObjectIdSlow(obj);
                    sceneObjects.Add(new MissingObjectData(scenePath, sceneName, obj.name, UNKNOWN, id));
                }
                else
                    CheckObjectProperties(scenePath, sceneName, obj, component);
            }
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                CheckGameObject(scenePath, sceneName, child);
            }
        }

        private void CheckObjectProperties(string assetPath, Object obj)
        {
            var serializedObject = new SerializedObject(obj);
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference
                    && serializedProperty.objectReferenceValue == null
                    && serializedProperty.objectReferenceInstanceIDValue != 0)
                {
                    assetObjects.Add(new MissingObjectData(assetPath, obj.name, obj.GetType().Name));
                }
            }
        }

        private void CheckObjectProperties(string scenePath, string sceneName, GameObject owner, Object obj)
        {
            var serializedObject = new SerializedObject(obj);
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference
                    && serializedProperty.objectReferenceValue == null
                    && serializedProperty.objectReferenceInstanceIDValue != 0)
                {
                    var id = GlobalObjectId.GetGlobalObjectIdSlow(owner);
                    sceneObjects.Add(new MissingObjectData(scenePath, sceneName, owner.name, obj.GetType().Name, id));
                }
            }
        }
    }
}