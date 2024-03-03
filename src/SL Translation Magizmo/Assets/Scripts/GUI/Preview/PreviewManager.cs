using Project.Translation.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Translation;
using Project.GUI.Hierarchy;
using qASIC.Files;

namespace Project.GUI.Preview
{
    public class PreviewManager : MonoBehaviour
    {
        [SerializeField] TranslationManager manager;
        [SerializeField] HierarchyController hierarchy;

        [EditorButton(nameof(PopulateVersions), activityType: ButtonActivityType.OnEditMode)]
        [EditorButton(nameof(SortVersions), activityType: ButtonActivityType.OnEditMode)]
        public List<PreviewVersion> versions = new List<PreviewVersion>();

        public PreviewVersion CurrentVersion { get; private set; }

        public TranslationManager TranslationManager =>
            manager;

        public HierarchyController Hierarchy =>
            hierarchy;

        public void ChangeVersion(Version version)
        {
            var previewVersion = versions.FirstOrDefault();
            foreach (var item in versions)
            {
                if (item.version == null) continue;
                if (item.version.version > version) break;
                previewVersion = item;
            }

            ChangeVersion(previewVersion);
        }

        public void ChangeVersion(PreviewVersion version)
        {
            PreviewScene previousScene = null;
            if (CurrentVersion != null)
            {
                previousScene = CurrentVersion.CurrentScene;
                CurrentVersion.gameObject.SetActive(false);
            }

            CurrentVersion = version;

            if (CurrentVersion != null)
            {
                CurrentVersion.gameObject.SetActive(true);
                if (previousScene != null)
                    CurrentVersion.SelectScene(previousScene.path);
            }
        }

        private void Awake()
        {
            foreach (var ver in versions)
            {
                ver.PreviewManager = this;
                ver.Initialize();
            }

            ChangeVersion(manager.CurrentVersion.version);

            manager.OnFileChanged += Manager_OnFileChanged;
            manager.OnCurrentVersionChanged += Manager_OnCurrentVersionChanged;
            hierarchy.OnSelect += Hierarchy_OnSelect;
        }

        private void Manager_OnCurrentVersionChanged(Translation.Mapping.TranslationVersion obj)
        {
            ChangeVersion(obj.version);
        }

        private void Hierarchy_OnSelect(HierarchyItem obj)
        {
            if (CurrentVersion == null) return;
            if (!(obj?.Item is SaveFile.EntryData entry)) return;

            if (CurrentVersion.CurrentScene.EntriesForIds.TryGetValue(entry.entryId, out var previewEntry))
            {
                previewEntry.ChangeTarget(entry.entryId);
                return;
            }

            if (CurrentVersion.ScenesForIds.TryGetValue(entry.entryId, out var newScene))
            {
                CurrentVersion?.SelectScene(newScene);
                newScene.EntriesForIds[entry.entryId].ChangeTarget(entry.entryId);
            }
        }

        private void Manager_OnFileChanged(object context)
        {
            if (context as Object == this) return;
            CurrentVersion?.Reload();
        }

        void PopulateVersions()
        {
            var a = FindObjectsByType<PreviewVersion>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Except(versions);

            versions.AddRange(a);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        void SortVersions()
        {
            versions = versions
                .OrderBy(x => x?.version?.version)
                .ToList();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}