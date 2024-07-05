using UnityEngine;
using UnityEngine.UIElements;
using qASIC.Options;
using System.Collections.Generic;
using System;
using SFB;
using System.Linq;
using qASIC;

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

            [Header("Apply On")]
            [InspectorLabel("Changed")] public bool applyOnChanged = true;
            [InspectorLabel("Mouse Up")] public bool applyOnMouseUp;
            [InspectorLabel("Focus Out")] public bool applyOnFocusOut;

            internal override void Initialize(VisualElement root)
            {
                element = root.Q<BaseField<T>>(elementName);
                //if (.TryGetOptionValue(targetOption, out object value) && value is T val)
                //    element.SetValueWithoutNotify(val);

                element.RegisterValueChangedCallback(args =>
                {
                    if (applyOnChanged)
                        Apply();
                });

                element.RegisterCallback<MouseCaptureOutEvent>(args =>
                {
                    if (applyOnMouseUp)
                        Apply();
                });

                element.RegisterCallback<FocusOutEvent>(args =>
                {
                    if (applyOnFocusOut)
                        Apply();
                });
            }

            protected void Apply()
            {
                //OptionsController.ChangeOption(targetOption, element.value);
            }
        }

        [Serializable] public class BoolTarget : Target<bool> { }
        [Serializable] public class StringTarget : Target<string> { }
        [Serializable] public class FloatTarget : Target<float> { }

        [Serializable]
        public class PathTarget : Target<string>
        {
            [Header("Path")]
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