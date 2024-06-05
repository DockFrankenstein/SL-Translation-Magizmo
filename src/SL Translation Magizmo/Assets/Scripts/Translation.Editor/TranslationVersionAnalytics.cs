using UnityEngine;
using UnityEditor;
using Project.Translation.Mapping;
using System.Collections.Generic;
using qASIC;
using System;
using System.Linq;

namespace Project.Translation.Editor
{
    
    public class TranslationVersionAnalytics : EditorWindow
    {
        [MenuItem("Window/Project/Translations/Analytics", priority = 1)]
        static TranslationVersionAnalytics CreateWindow()
        {
            var window = CreateInstance<TranslationVersionAnalytics>();
            window.titleContent = new GUIContent("Translation Analytics");
            window.minSize = new Vector2(500f, 200f);
            window.maxSize = new Vector2(500f, 200f);
            window.Show();
            return window;
        }

        [NonSerialized] bool _init = false;

        internal List<MappedField> defines = new List<MappedField>();

        TranslationVersion currentVersion;

        int totalFieldCount;
        int usedFieldCount;
        int ignoredFieldCount;
        int blankFieldCount;

        private void Initialize()
        {
            _init = true;

            RefreshMappings();
        }

        private void OnGUI()
        {
            if (!_init)
                Initialize();

            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                    RefreshMappings();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(currentVersion?.version.ToString() ?? "NONE", EditorStyles.toolbarButton))
                {
                    GenericMenu menu = new GenericMenu();

                    foreach (var item in TranslationProjectSettings.Instance.translationVersions)
                    {
                        menu.AddItem(item.version.ToString(), item == currentVersion, () =>
                        {
                            currentVersion = item;
                            RefreshMappings();
                        });
                    }

                    menu.ShowAsContext();
                }

                EditorGUILayout.Space();
            }

            using (new GUILayout.HorizontalScope(GUILayout.ExpandHeight(true)))
            {
                using (new GUILayout.VerticalScope(GUILayout.Width(60f)))
                    GUILayout.FlexibleSpace();

                if (Event.current.type == EventType.Repaint)
                {
                    var barRect = GUILayoutUtility.GetLastRect()
                        .Border(8f);

                    var barIgnoredRect = barRect
                        .SetHeight(barRect.height * (totalFieldCount == 0 ? 0 : (ignoredFieldCount / (float)totalFieldCount)));

                    var barBlankRect = barIgnoredRect
                        .SetHeight(barRect.height * (totalFieldCount == 0 ? 0 : (blankFieldCount / (float)totalFieldCount)))
                        .MoveY(barIgnoredRect.height);

                    var barUsedRect = barBlankRect
                        .SetHeight(barRect.height * (totalFieldCount == 0 ? 0 : (usedFieldCount / (float)totalFieldCount)))
                        .MoveY(barBlankRect.height);

                    new GUIStyle().WithBackgroundColor(Color.black)
                        .Draw(barRect, GUIContent.none, false, false, false, false);
                    new GUIStyle().WithBackgroundColor(new Color(252f / 255f, 45f / 255f, 73f / 255f))
                        .Draw(barIgnoredRect, GUIContent.none, false, false, false, false);
                    new GUIStyle().WithBackgroundColor(new Color(3f / 255f, 227f / 255f, 252f / 255f))
                        .Draw(barBlankRect, GUIContent.none, false, false, false, false);
                    new GUIStyle().WithBackgroundColor(new Color(3f / 255f, 252f / 255f, 161f / 255f))
                        .Draw(barUsedRect, GUIContent.none, false, false, false, false);
                }

                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField($"Total: {totalFieldCount}");
                    EditorGUILayout.LabelField(totalFieldCount == 0 ? "Ignored" : $"Ignored: {ignoredFieldCount} ({Mathf.Round(ignoredFieldCount / (float)totalFieldCount * 100)}%)");
                    EditorGUILayout.LabelField(totalFieldCount == 0 ? "Blank" : $"Blank: {blankFieldCount} ({Mathf.Round(blankFieldCount / (float)totalFieldCount * 100)}%)");
                    EditorGUILayout.LabelField(totalFieldCount == 0 ? "Used" : $"Used: {usedFieldCount} ({Mathf.Round(usedFieldCount / (float)totalFieldCount * 100)}%)");
                }
            }
        }

        void RefreshMappings()
        {
            totalFieldCount = 0;
            ignoredFieldCount = 0;
            usedFieldCount = 0;
            blankFieldCount = 0;

            if (currentVersion != null)
            {
                defines = currentVersion.containers
                    .SelectMany(x => x.GetAllMappedFields())
                    .ToList();

                totalFieldCount = defines.Count;
                usedFieldCount = defines.Where(x => x.Status == MappedField.SetupStatus.Used).Count();
                ignoredFieldCount = defines.Where(x => x.Status == MappedField.SetupStatus.Ignored).Count();
                blankFieldCount = defines.Where(x => x.Status == MappedField.SetupStatus.Blank).Count();
            }
        }
    }
}
