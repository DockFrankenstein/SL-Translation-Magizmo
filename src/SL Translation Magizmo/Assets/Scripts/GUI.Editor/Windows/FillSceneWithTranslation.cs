using UnityEditor;
using UnityEngine;
using Project.Translation.Data;
using qASIC;
using Project.Translation;
using System.Linq;
using Project.GUI.Preview;
using System.Collections.Generic;

namespace Project.GUI.Editor.Windows
{
    public class FillSceneWithTranslation : EditorWindow
    {
        [MenuItem("Window/Project/Preview/Fill With Translation")]
        static void OpenWindow()
        {
            var window = CreateWindow<FillSceneWithTranslation>("Fill Scene With Translation");
            window.Show();
        }

        SaveFileSerializer serializer = new SaveFileSerializer();
        [SerializeField] bool translationLoaded = false;
        [SerializeField] SaveFile translation;

        [SerializeField] bool _init = false;

        [SerializeField] bool clearEmpty = false;

        IEnumerable<PreviewEntry> Targets { get; set; } = new List<PreviewEntry>();

        public string TranslationName { get; private set; } = null;

        private void OnSelectionChange()
        {
            var sceneEntries = Selection.gameObjects
                .Select(x => x.GetComponent<PreviewScene>())
                .Where(x => x != null)
                .SelectMany(x => x.entries)
                .Where(x => x is PreviewEntry)
                .Select(x => x as PreviewEntry);

            var entries = Selection.gameObjects
                .Select(x => x.GetComponent<PreviewEntry>())
                .Where(x => x != null);

            Targets = sceneEntries
                .Concat(entries)
                .Distinct();

            Repaint();
        }

        private void OnEnable()
        {
            OnSelectionChange();
        }

        private void OnGUI()
        {
            if (!_init)
            {
                Clear();
                _init = true;
            }

            titleContent.text = translationLoaded ?
                $"Fill Scene ({TranslationName})" :
                "Fill Scene";

            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
                    Clear();

                GUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            EditorGUILayout.LabelField(GetTranslationStatus(),
                new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true, });

            if (GUILayout.Button("Open Translation"))
            {
                var transPath = EditorUtility.OpenFilePanel("Open Translation", Application.dataPath, SaveFile.FILE_EXTENSION);
                LoadTranslation(transPath);
            }

            EditorGUILayout.Space();

            clearEmpty = EditorGUILayout.Toggle(new GUIContent("Clear Empty", "Clear every entry default content even if it's not in the file."), clearEmpty);

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField(Targets.Count() == 0 ?
                "<b>Select elements to fill in the scene.</b>" :
                $"<b>Selected:</b>\n- {string.Join("\n- ", Targets)}", new GUIStyle(EditorStyles.label)
                {
                    richText = true,
                    wordWrap = true,
                    stretchHeight = true,
                });

            using (new EditorGUI.DisabledGroupScope(Targets.Count() == 0 || !translationLoaded))
            {
                if (GUILayout.Button("Fillout", GUILayout.Height(36f)))
                    Fillout();
            }
        }

        string GetTranslationStatus()
        {
            if (!translationLoaded)
                return "None Loaded";

            return $"<color=green>File Loaded</color>\n" +
                $"Name: {TranslationName}\n" +
                $"Entries: {translation.Entries.Count}";
        }

        void Clear()
        {
            translationLoaded = false;
            translation = null;
            TranslationName = null;
        }

        void Fillout()
        {
            foreach (var item in Targets)
            {
                FillItem(item.mainId.entryId, ref item.mainId.defaultValue);
                for (int i = 0; i < item.otherIds.Length; i++)
                    FillItem(item.otherIds[i].entryId, ref item.otherIds[i].defaultValue);

                item.OnValidate();
                EditorUtility.SetDirty(item);
            }

            void FillItem(string id, ref string content)
            {
                if (clearEmpty)
                    content = string.Empty;

                if (translation.Entries.TryGetValue(id.ToLower(), out var item))
                    content = item.content;
            }
        }

        void LoadTranslation(string path)
        {
            translation = serializer.Load(path);
            translationLoaded = true;

            var slVersion = translation.UseNewestSlVersion ?
                TranslationProjectSettings.Instance.NewestVersion :
                translation.SlVersion;

            var field = TranslationProjectSettings.Instance.translationVersions
                .Where(x => x.version == slVersion)
                .FirstOrDefault()?.GetNameField();

            TranslationName = field != null && translation.Entries.TryGetValue(field.id, out SaveFile.EntryData nameData) ?
                nameData.content :
                "Unknown";
        }
    }
}