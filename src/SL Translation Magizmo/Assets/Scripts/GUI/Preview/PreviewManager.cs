using Project.Translation.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Translation;
using Project.GUI.Hierarchy;
using Project.Translation.Mapping;
using System;

using UObject = UnityEngine.Object;
using qASIC.Input;
using System.Collections;
using qASIC;

namespace Project.GUI.Preview
{
    public class PreviewManager : MonoBehaviour
    {
        [Label("References")]
        [SerializeField] TranslationManager manager;
        [SerializeField] HierarchyController hierarchy;

        [Label("Scenes")]
        [SerializeField] Transform sceneHolder;
        [EditorButton(nameof(AutoDetectScenes), "Populate")]
        [SerializeField] List<PreviewScene> scenes = new List<PreviewScene>();

        [Label("Input")]
        [SerializeField] float repeatWaitTime = 1f;
        [SerializeField] float repeatTime = 0.1f;
        [SerializeField] InputMapItemReference i_previous;
        [SerializeField] InputMapItemReference i_next;

        public Dictionary<string, PreviewScene> ScenesForIds { get; private set; } = new Dictionary<string, PreviewScene>();
        public List<PreviewScene> VersionScenes { get; private set; } = new List<PreviewScene>();
        public PreviewScene CurrentScene { get; private set; }

        public Version CurrentVersion { get; private set; }

        public TranslationManager TranslationManager =>
            manager;

        public HierarchyController Hierarchy =>
            hierarchy;

        public event Action OnChangeScene;

        public void ChangeVersion(Version version)
        {
            CurrentVersion = version;

            Dictionary<string, PreviewScene> newScenes = new Dictionary<string, PreviewScene>();
            VersionScenes.Clear();
            string selectedPath = CurrentScene?.path;

            //Disable selected scene
            if (CurrentScene != null)
                CurrentScene.gameObject.SetActive(false);

            //Generate scene list
            //This includes scenes for this version and
            //older. We use the newest scene avaliable
            foreach (var item in scenes)
            {
                if (item == null) continue;

                //Ignore if scene is for a newer version
                if (item.version > version)
                    continue;

                var path = item.path.ToLower();
                if (!newScenes.ContainsKey(path))
                {
                    newScenes.Add(path, item);
                    continue;
                }

                if (newScenes[path].version < item.version)
                    newScenes[path] = item;
            }

            VersionScenes = newScenes
                .Select(x => x.Value)
                .ToList();

            ScenesForIds = scenes
                .SelectMany(x => x.EntriesForIds.Select(y => new KeyValuePair<string, PreviewScene>(y.Key, x)))
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.First().Value);

            SelectScene(TryGetSceneByPath(selectedPath, out var scene) ? scene : VersionScenes[0]);
        }

        private void Awake()
        {
            foreach (var scene in scenes)
            {
                if (scene == null) continue;
                scene.gameObject.SetActive(false);

                scene.manager = TranslationManager;
                scene.hierarchy = Hierarchy;

                scene.Initialize();
            }

            ChangeVersion(manager.CurrentVersion.version);

            manager.OnFileChanged += Manager_OnFileChanged;
            manager.OnCurrentVersionChanged += Manager_OnCurrentVersionChanged;
            hierarchy.OnSelect += Hierarchy_OnSelect;

            StartCoroutine(HandleInput());
        }

        float _inputPressTime;
        InputEventType _previousInput;
        InputEventType _nextInput;

        private void Update()
        {
            _previousInput = i_previous.GetInputEvent();
            _nextInput = i_next.GetInputEvent();

            var inputs = new InputEventType[]
            {
                _previousInput,
                _nextInput,
            };

            if (inputs.Where(x => x.HasFlag(InputEventType.Pressed) && !x.HasFlag(InputEventType.Down)).Count() != 1)
            {
                _inputPressTime = 0f;
                return;
            }

            _inputPressTime += Time.deltaTime;
        }

        IEnumerator HandleInput()
        {
            while (true)
            {
                RunInput();
                yield return null;

                if (_inputPressTime == 0f)
                    continue;

                while (_inputPressTime != 0f)
                {
                    yield return new WaitForSecondsRealtime(repeatTime);

                    if (_inputPressTime < repeatWaitTime)
                        continue;

                    RunInput();
                }
            }

            void RunInput()
            {
                if (_previousInput.HasFlag(InputEventType.Pressed))
                    SelectBy(-1);

                if (_nextInput.HasFlag(InputEventType.Pressed))
                    SelectBy(1);
            }
        }

        private void Manager_OnCurrentVersionChanged(TranslationVersion obj)
        {
            ChangeVersion(obj.version);
        }

        private void Hierarchy_OnSelect(HierarchyItem obj)
        {
            if (CurrentVersion == null) return;
            if (!(obj?.Item is SaveFile.EntryData entry)) return;

            if (CurrentScene.EntriesForIds.TryGetValue(entry.entryId, out var previewEntry))
            {
                previewEntry.ChangeTarget(entry.entryId);
                return;
            }

            if (ScenesForIds.TryGetValue(entry.entryId, out var newScene))
            {
                SelectScene(newScene);
                newScene.EntriesForIds[entry.entryId].ChangeTarget(entry.entryId);
            }
        }

        private void Manager_OnFileChanged(object context)
        {
            if (context as UObject == this) return;
            Reload();
        }

        public void Reload()
        {
            if (CurrentScene == null) return;
            CurrentScene.Reload();
        }

        public void SelectBy(int amount)
        {
            if (amount == 0)
                return;

            if (VersionScenes.Count == 0)
                return;

            var index = VersionScenes.IndexOf(CurrentScene);
            index += amount;

            while (index < 0)
                index += VersionScenes.Count;

            while (index >= VersionScenes.Count)
                index -= VersionScenes.Count;

            SelectScene(VersionScenes[index]);
        }

        public void SelectScene(string path)
        {
            if (TryGetSceneByPath(path, out var scene))
                SelectScene(scene);
        }

        public bool TryGetSceneByPath(string path, out PreviewScene scene)
        {
            path = path?.ToLower();

            scene = VersionScenes
                .Where(x => x.path.ToLower() == path)
                .FirstOrDefault();

            return scene != null;
        }

        public void SelectScene(PreviewScene scene)
        {
            if (CurrentScene != null)
                CurrentScene.gameObject.SetActive(false);

            CurrentScene = scene;

            if (CurrentScene != null)
                CurrentScene.gameObject.SetActive(true);

            OnChangeScene?.Invoke();
        }

        void AutoDetectScenes()
        {
            var detected = sceneHolder.GetComponentInShallowChildren<PreviewScene>();

            scenes = scenes
                .Except(detected)
                .Where(x => x != null)
                .Concat(detected)
                .ToList();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}