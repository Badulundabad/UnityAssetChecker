using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetChecker
{
    public class AssetCheckerResult : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<Object> list;

        public static EditorWindow ShowResult(List<Object> list)
        {
            var window = EditorWindow.GetWindow<AssetCheckerResult>("Asset Checker Result");
            window.SetContext(list);
            window.minSize = new Vector2(300, 200);
            return window;
        }

        private void SetContext(List<Object> list)
        {
            this.list = list;
        }

        private void OnGUI()
        {
            DrawRefList();
        }

        private void DrawRefList()
        {
            if (list != null)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox);
                foreach (var obj in list)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{path}", EditorStyles.boldLabel, GUILayout.Height(25));
                    if (GUILayout.Button($"{obj.GetType().Name}", EditorStyles.textArea, GUILayout.MaxWidth(160), GUILayout.Height(25)))
                    {
                        ProjectWindowUtil.ShowCreatedAsset(obj);
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}