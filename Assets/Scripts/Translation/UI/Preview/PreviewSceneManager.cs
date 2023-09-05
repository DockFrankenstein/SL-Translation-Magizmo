using System;
using UnityEngine;

namespace Project.Translation.UI.Preview
{
    public class PreviewSceneManager : MonoBehaviour
    {
        [SerializeField] TranslationManager manager;
        [SerializeField] HierarchyDisplay hierarchy;

        public PreviewScene[] scenes;

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