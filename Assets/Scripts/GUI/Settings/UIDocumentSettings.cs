using UnityEngine;
using UnityEngine.UIElements;
using qASIC.SettingsSystem;
using System.Collections.Generic;
using System;
using SFB;

namespace Project.GUI.Settings
{
    public class UIDocumentSettings : MonoBehaviour
    {
        public UIDocument document;

        [SerializeReference, Subclass(InIsList: true)] 
        public List<Target> elements = new List<Target>();

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var root = document.rootVisualElement;
            foreach (var item in elements)
                item.Initialize(root);
        }

        [Serializable]
        public abstract class Target
        {
            public string elementName;
            public string targetOption;

            public virtual Type ValueType => null;

            internal virtual void Initialize(VisualElement root)
            {

            }
        }

        [Serializable]
        public class Target<T> : Target
        {
            protected BaseField<T> element;

            public override Type ValueType => typeof(T);

            internal override void Initialize(VisualElement root)
            {
                element = root.Q<BaseField<T>>(elementName);
                if (OptionsController.TryGetOptionValue(targetOption, out object value) && value is T val)
                    element.SetValueWithoutNotify(val);

                element.RegisterValueChangedCallback(args =>
                {
                    OptionsController.ChangeOption(targetOption, element.value);
                });
            }
        }

        [Serializable] public class BoolTarget : Target<bool> { }
        [Serializable] public class StringTarget : Target<string> { }
        [Serializable] public class FloatTarget : Target<float> { }

        [Serializable]
        public class PathTarget : Target<string>
        {
            public string openButtonName;

            Button _openButton;

            internal override void Initialize(VisualElement root)
            {
                base.Initialize(root);

                _openButton = root.Q<Button>(openButtonName);

                _openButton.clicked += () =>
                {
                    var paths = StandaloneFileBrowser.OpenFolderPanel("", element.value, false);

                    if (paths.Length == 0)
                        return;

                    element.value = paths[0];
                };
            }
        }
    }
}