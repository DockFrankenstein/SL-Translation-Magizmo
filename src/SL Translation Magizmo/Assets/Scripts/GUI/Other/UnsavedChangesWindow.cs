using Project.Translation;
using Project.Undo;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Other
{
    public class UnsavedChangesWindow : MonoBehaviour
    {
        [SerializeField] TranslationManager manager;
        [SerializeField] UndoManager undo;
        [SerializeField] UIDocument document;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
            manager = FindObjectOfType<TranslationManager>();
            undo = FindObjectOfType<UndoManager>();
        }

        Button _cancel;
        Button _discard;
        Button _save;

        Action _onContinue;

        private void Awake()
        {
            var root = document.rootVisualElement;
            root.ChangeDispaly(false);

            _cancel = root.Q<Button>("cancel");
            _discard = root.Q<Button>("discard");
            _save = root.Q<Button>("save");

            _cancel.clicked += () =>
            {
                root.ChangeDispaly(false);
                _onContinue = null;
            };

            _discard.clicked += () =>
            {
                root.ChangeDispaly(false);
                _onContinue?.Invoke();
                _onContinue = null;
            };

            _save.clicked += () =>
            {
                root.ChangeDispaly(false);
                manager.OnSave.AddListener(OnSave);
                manager.OnCancelSave.AddListener(OnCancelSave);

                manager.Save();
            };

            undo.OnChanged += Undo_OnChanged;
        }

        void Undo_OnChanged(object context)
        {
            PUtility.ChangeWindowTitle($"{(undo.IsDirty ? "*" : "")}{Application.productName}");
        }

        private void OnEnable()
        {
            Application.wantsToQuit += WantsToQuit;
            manager.OnWantToLoad += WantsToLoad;
        }

        private void OnDisable()
        {
            Application.wantsToQuit -= WantsToQuit;
            manager.OnWantToLoad -= WantsToLoad;
        }

        void OnSave()
        {
            _onContinue?.Invoke();
            _onContinue = null;
        }

        void OnCancelSave()
        {
            manager.OnSave.RemoveListener(OnSave);
            manager.OnCancelSave.RemoveListener(OnCancelSave);
            _onContinue = null;
        }

        bool _forceLoad;
        string _lastPath;
        bool WantsToLoad(string path)
        {
            _lastPath = path;

            if (!_forceLoad && undo.IsDirty)
            {
                document.rootVisualElement.ChangeDispaly(true);
                _onContinue += () =>
                {
                    _forceLoad = true;
                    manager.Open(path);
                };

                return false;
            }

            _forceLoad = false;
            return true;
        }

        bool _forceQuit;
        bool WantsToQuit()
        {
            if (!_forceQuit && undo.IsDirty)
            {
                document.rootVisualElement.ChangeDispaly(true);
                _onContinue += () =>
                {
                    _forceQuit = true;
                    Application.Quit();
                };

                return false;
            }

            _forceQuit = false;
            return true;
        }
    }
}