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
                    RunAssetsCheck();
                    RunSceneObjectsCheck();
                    isCheckInProgress = false;
                    ShowResult();
                }
            }
        }

        private void ShowResult()
        {
            resultWindow = AssetCheckerResult.ShowResult(assetObjects, sceneObjects);
        }

        private void RunAssetsCheck()
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

        private void RunSceneObjectsCheck()
        {
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scnConf = EditorBuildSettings.scenes[i];
                Scene scene = EditorSceneManager.OpenScene(scnConf.path);
                GameObject[] sceneRootObjects = scene.GetRootGameObjects();
                for (int j = 0; j < sceneRootObjects.Length; j++)
                {
                    GameObject obj = sceneRootObjects[j];
                    CheckGameObject(scene.name, scene.path, obj, j);
                }
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
                    CheckComponentProperties(assetPath, component);
            }
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                CheckGameObject(assetPath, child);
            }
        }

        private void CheckGameObject(string sceneName, string scenePath, GameObject obj, int objIndex)
        {
            var components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (!component)
                    sceneObjects.Add(new MissingObjectData(scenePath, sceneName, obj.name, UNKNOWN, objIndex));
                else
                    CheckComponentProperties(scenePath, sceneName, obj.name, component, objIndex);
            }
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                CheckGameObject(scenePath, sceneName, child, objIndex);
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
                    assetObjects.Add(new MissingObjectData(assetPath, comp.name, comp.GetType().Name));
                }
            }
        }

        private void CheckComponentProperties(string scenePath, string sceneName, string ownerName, Component comp, int objIndex)
        {
            var serializedObject = new SerializedObject(comp);
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference
                    && serializedProperty.objectReferenceValue == null
                    && serializedProperty.objectReferenceInstanceIDValue != 0)
                {
                    sceneObjects.Add(new MissingObjectData(scenePath, sceneName, ownerName, comp.GetType().Name, objIndex));
                }
            }
        }
    }
}