using UnityEngine;
using System.Linq;
using Project.Utility.UI;
using System.Collections.Generic;
using Project.GUI.Preview.Interfaces;
using Project.GUI.Hierarchy;
using Project.Translation;

namespace Project.GUI.Preview
{
    public class PreviewScene : MonoBehaviour
    {
        public string path;
        public Version version;

        [Space]
        [EditorButton(nameof(AutoDetectEntries), "Populate")]
        public PreviewElement[] entries = new PreviewElement[0];

        [EditorButton(nameof(AutoDetectScenes), "Populate")]
        [SerializeField] PreviewScene[] scenes;

        [HideInInspector] public TranslationManager manager;
        [HideInInspector] public HierarchyController hierarchy;

        public PreviewScene ParentScene { get; internal set; } = null;
        public PreviewScene[] ChildScenes => scenes;

        public bool Interactable { get; set; } = true;

        public Dictionary<string, IHasMappedTargets> EntriesForIds { get; private set; } = new Dictionary<string, IHasMappedTargets>();

        /// <summary>Initializes the scene. Use this when switching.</summary>
        /// <param name="embeded">Is the scene loaded by another scene?</param>
        public void Initialize(bool embeded = false)
        {
            EntriesForIds = entries
                .Where(x => x is IHasMappedTargets)
                .Select(x => x as IHasMappedTargets)
                .SelectMany(x => x.GetListOfTargets().Select(y => new KeyValuePair<string, IHasMappedTargets>(y.entryId, x)))
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.First().Value);

            Interactable = !embeded;

            if (!embeded)
            {
                ParentScene = null;
            }

            foreach (var item in scenes)
            {
                item.ParentScene = this;
                item.manager = manager;
                item.hierarchy = hierarchy;
                item?.Initialize(true);
            }

            foreach (var item in entries)
            {
                item.manager = manager;
                item.hierarchy = hierarchy;
                item.scene = this;
            }
        }

        /// <summary>Updates entries and other items.</summary>
        public void Reload()
        {
            ReloadContent();
            DelayExecute.NextFrame(() =>
            {
                LayoutGroupController.Refresh();
            });
        }

        internal void ReloadContent()
        {
            foreach (var entry in entries)
                entry?.Reload();

            foreach (var scene in scenes)
                scene?.ReloadContent();
        }

        private void OnEnable()
        {
            Reload();
        }

        void AutoDetectEntries()
        {
#if UNITY_EDITOR
            var detectedEntries = transform.GetComponentInShallowChildren<PreviewElement>(new System.Type[] { typeof(PreviewScene) })
                .Except(entries);

            entries = entries
                .Where(x => x != null)
                .Concat(detectedEntries)
                .ToArray();

            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        void AutoDetectScenes()
        {
#if UNITY_EDITOR
            var detectedScenes = transform.GetComponentInShallowChildren<PreviewScene>()
                .Except(scenes);

            scenes = scenes
                .Where(x => x != null)
                .Concat(detectedScenes)
                .ToArray();

            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}