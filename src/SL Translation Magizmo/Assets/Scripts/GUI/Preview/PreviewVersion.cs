using Project.GUI.Hierarchy;
using Project.Translation.Mapping;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.GUI.Preview
{
    public class PreviewVersion : MonoBehaviour
    {
        public TranslationVersion version;
        public List<PreviewScene> scenes = new List<PreviewScene>();

        public PreviewScene CurrentScene { get; private set; }

        public Dictionary<string, PreviewScene> ScenesForIds { get; private set; } = new Dictionary<string, PreviewScene>();
        public PreviewManager PreviewManager { get; internal set; }

        internal void Initialize()
        {
            foreach (var scene in scenes)
            {
                scene.gameObject.SetActive(false);
                foreach (var entry in scene.entries)
                {
                    entry.manager = PreviewManager.TranslationManager;
                    entry.hierarchy = PreviewManager.Hierarchy;
                }

                scene.Initialize();
            }

            ScenesForIds = scenes
                .SelectMany(x => x.EntriesForIds.Select(y => new KeyValuePair<string, PreviewScene>(y.Key, x)))
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.First().Value);

            if (scenes.Count > 0)
                SelectScene(scenes[0]);
        }

        private void OnEnable()
        {
            if (CurrentScene == null)
                SelectScene(scenes.FirstOrDefault());
        }

        private void OnDisable()
        {
            SelectScene(null as PreviewScene);
        }

        public void Reload()
        {
            if (CurrentScene == null) return;
            CurrentScene.Reload();
        }

        public void SelectScene(string path)
        {
            path = path?.ToLower();

            var target = scenes
                .Where(x => x.path.ToLower() == path)
                .FirstOrDefault();

            SelectScene(target);
        }

        public void SelectScene(PreviewScene scene)
        {
            if (CurrentScene != null)
                CurrentScene.gameObject.SetActive(false);

            CurrentScene = scene;

            if (CurrentScene != null)
                CurrentScene.gameObject.SetActive(true);
        }
    }
}