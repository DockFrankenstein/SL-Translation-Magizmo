using Project.Translation.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Translation;
using Project.GUI.Hierarchy;
using Project.Translation.Mapping;

namespace Project.GUI.Preview
{
    public class PreviewSceneManager : MonoBehaviour
    {
        [SerializeField] TranslationManager manager;
        [SerializeField] HierarchyController hierarchy;

#if UNITY_EDITOR
        [EditorButton(nameof(PopulateScenes))]
#endif
        public List<PreviewScene> scenes = new List<PreviewScene>();

        public PreviewScene CurrentScene { get; private set; }

        public Dictionary<string, PreviewScene> ScenesForIds { get; private set; } = new Dictionary<string, PreviewScene>();

        public void ReloadActiveScenes()
        {
            var targetScenes = scenes
                .Where(x => x.enabled == true);

            foreach (var scene in targetScenes)
                scene.UpdateScene();
        }

#if UNITY_EDITOR
        void PopulateScenes()
        {
            var detectedEntries = GetComponentsInChildren<PreviewScene>(true)
                .Except(scenes);

            if (detectedEntries.Count() == 0) return;

            scenes = scenes
                .Concat(detectedEntries)
                .ToList();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        private void Awake()
        {
            foreach (var scene in scenes)
            {
                scene.gameObject.SetActive(false);
                foreach (var entry in scene.entries)
                {
                    entry.manager = manager;
                    entry.hierarchy = hierarchy;
                }
            }

            if (scenes.Count > 0)
                SelectScene(scenes[0]);

            manager.OnFileChanged += Manager_OnFileChanged;
            hierarchy.OnSelect += Hierarchy_OnSelect;
        }

        private void Hierarchy_OnSelect(HierarchyItem obj)
        {
            if (!(obj.Item is SaveFile.EntryData entry)) return;
            if (CurrentScene.EntriesForIds.TryGetValue(entry.entryId, out var previewEntry))
            {
                previewEntry.ChangeTarget(entry.entryId);
                return;
            }
        }

        private void Manager_OnFileChanged()
        {
            ReloadActiveScenes();
        }

        public void SelectScene(PreviewScene scene)
        {
            if (CurrentScene != null)
                CurrentScene.gameObject.SetActive(false);

            CurrentScene = scene;
            scene.gameObject.SetActive(true);
        }
    }
}