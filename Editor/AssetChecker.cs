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
        private List<AssetObjectData> foundAssetObjects;
        private List<SceneObjectData> sceneObjects;
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
                    foundAssetObjects = new List<AssetObjectData>();
                    sceneObjects = new List<SceneObjectData>();
                    CheckInBuildScenes();
                    RunAssetCheck();
                    isCheckInProgress = false;
                    ShowResult();
                }
            }
        }

        private void ShowResult()
        {
            resultWindow = AssetCheckerResult.ShowResult(foundAssetObjects, sceneObjects);
        }

        private void RunAssetCheck()
        {
            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj)
                {
                    CheckGameObject(path, obj);
                }
            }
        }

        private void CheckGameObject(string path, GameObject obj)
        {
            var components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (!component)
                    foundAssetObjects.Add(new AssetObjectData(path, obj.name, UNKNOWN));
                else
                    CheckComponentProperties(path, component);
            }
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                CheckGameObject(path, child);
            }
        }

        private void CheckComponentProperties(string assetPath, Component comp)
        {
            var serializedObject = new SerializedObject(comp);
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference
                    && serializedProperty.objectReferenceValue == null
                    && serializedProperty.objectReferenceInstanceIDValue != 0)
                {
                    foundAssetObjects.Add(new AssetObjectData(assetPath, comp.name, comp.GetType().Name));
                }
            }
        }

        private void CheckInBuildScenes()
        {
            foreach (var scnConf in EditorBuildSettings.scenes)
            {
                Scene scene = EditorSceneManager.OpenScene(scnConf.path);
                foreach (GameObject obj in scene.GetRootGameObjects())
                {
                    CheckSceneGameObject(scene.name, scene.path, obj);
                }
            }
        }

        private void CheckSceneGameObject(string sceneName, string scenePath, GameObject obj)
        {
            var components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (!component)
                    sceneObjects.Add(new SceneObjectData(sceneName, scenePath, obj.name, UNKNOWN));
                else
                    CheckSceneComponentProperties(sceneName, scenePath, obj.name, component);
            }
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                CheckSceneGameObject(sceneName, scenePath, child);
            }
        }

        private void CheckSceneComponentProperties(string sceneName, string scenePath, string ownerName, Component comp)
        {
            var serializedObject = new SerializedObject(comp);
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference
                    && serializedProperty.objectReferenceValue == null
                    && serializedProperty.objectReferenceInstanceIDValue != 0)
                {
                    sceneObjects.Add(new SceneObjectData(sceneName, scenePath, ownerName, comp.GetType().Name));
                }
            }
        }
    }
}