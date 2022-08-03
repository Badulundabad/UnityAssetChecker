using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        private List<MissingObjectData> assetObjects;
        private List<MissingObjectData> sceneObjects;

        public static EditorWindow ShowResult(List<MissingObjectData> assetObjects, List<MissingObjectData> sceneObjects)
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

                if (assetObjects != null && assetObjects.Count > 0)
                {
                    isAssetFoldoutOn = EditorGUILayout.Foldout(isAssetFoldoutOn, "Asset Objects");
                    if (isAssetFoldoutOn)
                        DrawAssetObjectList();
                }

                if (sceneObjects != null && sceneObjects.Count > 0)
                {
                    isSceneFoldoutOn = EditorGUILayout.Foldout(isSceneFoldoutOn, "Scene Objects");
                    if (isSceneFoldoutOn)
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
                MissingObjectData data = assetObjects[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{data.ObjectName}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                GUILayout.Label($"{data.Component}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                GUILayout.Label("", GUILayout.Height(25), GUILayout.MinWidth(100)); // Just for better UI
                if (GUILayout.Button(buttonImage, GUILayout.Height(25), GUILayout.Width(25)))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(data.ObjectPath);
                    if (obj)
                    {
                        if (data.Component == AssetChecker.UNKNOWN)
                        {
                            AssetDatabase.OpenAsset(obj);
                        }
                        else
                        {
                            ProjectWindowUtil.ShowCreatedAsset(obj);
                        }
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
                MissingObjectData objectData = sceneObjects[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{objectData.SceneName}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                GUILayout.Label($"{objectData.ObjectName}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                GUILayout.Label($"{objectData.Component}", EditorStyles.boldLabel, GUILayout.Height(25), GUILayout.MinWidth(100));
                if (GUILayout.Button(buttonImage, GUILayout.Height(25), GUILayout.Width(25)))
                {
                    Scene scene = EditorSceneManager.GetActiveScene();
                    if (scene.path != objectData.ScenePath)
                    {
                        if (EditorUtility.DisplayDialog("Warning", $"Load {objectData.SceneName} scene?", "Yes", "No"))
                        {
                            EditorSceneManager.OpenScene(objectData.ScenePath, OpenSceneMode.Single);
                            Object obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(objectData.ObjectId);
                            if (obj)
                                ProjectWindowUtil.ShowCreatedAsset(obj);
                        }
                    }
                    else
                    {
                        Object obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(objectData.ObjectId);
                        if (obj)
                            ProjectWindowUtil.ShowCreatedAsset(obj);
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}