using Project.Translation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Other
{
    public class UnsavedChangesWindow : MonoBehaviour
    {
        [SerializeField] TranslationManager manager;
        [SerializeField] UIDocument document;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
            manager = FindObjectOfType<TranslationManager>();
        }

        Button _cancel;
        Button _discard;
        Button _save;

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
            };

            _discard.clicked += () =>
            {
                _forceQuit = true;
                Application.Quit();
            };

            _save.clicked += () =>
            {
                root.ChangeDispaly(false);
                manager.OnSave.AddListener(CloseAfterSave);
                manager.OnCancelSave.AddListener(OnCloseAfterSaveSaveFail);

                manager.Save();
            };
        }

        private void OnEnable()
        {
            Application.wantsToQuit += WantsToQuit;
        }

        private void OnDisable()
        {
            Application.wantsToQuit -= WantsToQuit;
        }

        void CloseAfterSave()
        {
            Application.Quit();
        }

        void OnCloseAfterSaveSaveFail()
        {
            manager.OnSave.RemoveListener(CloseAfterSave);
            manager.OnCancelSave.RemoveListener(OnCloseAfterSaveSaveFail);
        }

        bool _forceQuit;
        bool WantsToQuit()
        {
            if (!_forceQuit && manager.IsDirty)
            {
                document.rootVisualElement.ChangeDispaly(true);
                return false;
            }

            return true;
        }
    }
}