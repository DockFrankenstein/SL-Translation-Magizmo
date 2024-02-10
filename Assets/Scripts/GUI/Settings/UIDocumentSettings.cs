using UnityEngine;
using UnityEngine.UIElements;
using qASIC.SettingsSystem;
using System.Collections.Generic;
using System;

namespace Project.GUI.Settings
{
    public class UIDocumentSettings : MonoBehaviour
    {
        public UIDocument document;

#if UNITY_EDITOR
        [EditorButton(nameof(AddBoolElement))]
        [EditorButton(nameof(AddStringElement))]
        [EditorButton(nameof(AddFloatElement))]
#endif
        [SerializeReference]
        public List<Target> elements = new List<Target>();

#if UNITY_EDITOR
        void AddBoolElement()
        {
            elements.Add(new BoolTarget());
        }

        void AddStringElement()
        {
            elements.Add(new StringTarget());
        }

        void AddFloatElement()
        {
            elements.Add(new FloatTarget());
        }
#endif

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
            BaseField<T> _element;

            public override Type ValueType => typeof(T);

            internal override void Initialize(VisualElement root)
            {
                _element = root.Q<BaseField<T>>(elementName);
                if (OptionsController.TryGetOptionValue(targetOption, out object value) && value is T val)
                    _element.SetValueWithoutNotify(val);

                _element.RegisterValueChangedCallback(args =>
                {
                    OptionsController.ChangeOption(targetOption, _element.value);
                });
            }
        }

        [Serializable] public class BoolTarget : Target<bool> { }
        [Serializable] public class StringTarget : Target<string> { }
        [Serializable] public class FloatTarget : Target<float> { }
    }
}