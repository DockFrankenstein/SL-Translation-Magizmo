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

        public PreviewScene[] scenes;

        public void ReloadActiveScenes(AppFile appFile)
        {
            var targetScenes = scenes
                .Where(x => x.enabled == true);

            foreach (var scene in targetScenes)
                scene.ReloadScene(appFile);
        }

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