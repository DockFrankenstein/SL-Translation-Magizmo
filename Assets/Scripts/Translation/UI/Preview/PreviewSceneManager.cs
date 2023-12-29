using Project.Translation.Data;
using System;
using UnityEngine;
using System.Linq;

namespace Project.Translation.UI.Preview
{
    public class PreviewSceneManager : MonoBehaviour
    {
        [SerializeField] TranslationManager manager;
        [SerializeField] HierarchyDisplay hierarchy;

#if UNITY_EDITOR
        [EditorButton(nameof(PopulateScenes))]
#endif
        public PreviewScene[] scenes = new PreviewScene[0];

        public void ReloadActiveScenes(AppFile appFile)
        {
            var targetScenes = scenes
                .Where(x => x.enabled == true);

            foreach (var scene in targetScenes)
                scene.ReloadScene(appFile);
        }

#if UNITY_EDITOR
        void PopulateScenes()
        {
            var detectedEntries = GetComponentsInChildren<PreviewScene>(true)
                .Except(scenes);

            if (detectedEntries.Count() == 0) return;

            scenes = scenes
                .Concat(detectedEntries)
                .ToArray();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        private void Awake()
        {
            foreach (var scene in scenes)
            {
                foreach (var entry in scene.entries)
                {
                    entry.manager = manager;
                    entry.hierarchy = hierarchy;
                }
            }
        }
    }
}