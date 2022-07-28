using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class AssetCheckerResult : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<KeyValuePair<string, string>> list;

        public static EditorWindow ShowResult(List<KeyValuePair<string, string>> list)
        {
            var window = EditorWindow.GetWindow<AssetCheckerResult>("Asset Checker Result");
            window.SetContext(list);
            window.minSize = new Vector2(300, 200);
            return window;
        }

        private void SetContext(List<KeyValuePair<string, string>> list)
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
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                foreach (var kvp in list)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{kvp.Key}", EditorStyles.objectField);
                    EditorGUILayout.LabelField($"{kvp.Value}", EditorStyles.objectField, GUILayout.MaxWidth(160));
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}