using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AssetChecker
{
    public class AssetCheckerResult : EditorWindow
    {
        [SerializeField] private Texture buttonImage;
        private bool isAssetFoldoutOn;
        private bool isSceneFoldoutOn;
        private Vector2 mainScrollPos;
        private Vector2 assetScrollPos;
        private Vector2 sceneScrollPos;
        private bool canShowContent;
        private List<AssetObjectData> assetObjects;
        private List<SceneObjectData> sceneObjects;

        public static EditorWindow ShowResult(List<AssetObjectData> assetObjects, List<SceneObjectData> sceneObjects)
        {
            var window = EditorWindow.GetWindow<AssetCheckerResult>("Asset Checker Result");
            window.assetObjects = assetObjects;
            window.sceneObjects = sceneObjects;
            window.minSize = new Vector2(365, 200);
            window.canShowContent = true;
            return window;
        }

        private void OnGUI()
        {
            if (canShowContent)
            {
                mainScrollPos = EditorGUILayout.BeginScrollView(mainScrollPos);

                isAssetFoldoutOn = EditorGUILayout.Foldout(isAssetFoldoutOn, "Asset Objects");
                if (assetObjects != null && assetObjects.Count > 0 && isAssetFoldoutOn)
                {
                    DrawAssetObjectList();
                }

                isSceneFoldoutOn = EditorGUILayout.Foldout(isSceneFoldoutOn, "Scene Objects");
                if (sceneObjects != null && sceneObjects.Count > 0 && isSceneFoldoutOn)
                {
                    DrawSceneObjectList();
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawAssetObjectList()
        {
            assetScrollPos = EditorGUILayout.BeginScrollView(assetScrollPos, EditorStyles.helpBox);
            for (int i = 0; i < assetObjects.Count; i++)
            {
                AssetObjectData data = assetObjects[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{data.Owner}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                GUILayout.Label($"{data.Component}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                GUILayout.Label("", GUILayout.Height(25), GUILayout.MinWidth(100)); // Just for better UI
                if (GUILayout.Button(buttonImage, GUILayout.Height(25), GUILayout.Width(25)))
                {                   
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(data.AssetPath);
                    if (data.Component == AssetChecker.UNKNOWN)
                    {
                        AssetDatabase.OpenAsset(obj);
                    }
                    else
                    {
                        ProjectWindowUtil.ShowCreatedAsset(obj);
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawSceneObjectList()
        {
            sceneScrollPos = EditorGUILayout.BeginScrollView(sceneScrollPos, EditorStyles.helpBox);
            for (int i = 0; i < sceneObjects.Count; i++)
            {
                SceneObjectData objectData = sceneObjects[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{objectData.SceneName}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                GUILayout.Label($"{objectData.Owner}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                GUILayout.Label($"{objectData.Component}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                if (GUILayout.Button(buttonImage, GUILayout.Height(25), GUILayout.Width(25)))
                {
                    if (EditorUtility.DisplayDialog("Warning", $"Load {objectData.SceneName} scene?", "Yes", "No"))
                    {
                        EditorSceneManager.OpenScene(objectData.ScenePath, OpenSceneMode.Single);
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}