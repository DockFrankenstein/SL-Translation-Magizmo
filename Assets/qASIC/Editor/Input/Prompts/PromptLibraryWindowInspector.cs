using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using qASIC.EditorTools;

namespace qASIC.Input.Prompts.Internal
{
    [Serializable]
    public class PromptLibraryWindowInspector
    { 
        public PromptLibraryWindowInspector()
        {

        }

        PromptLibraryWindow window;

        Type _selectedObjectsType;
        bool _noneSelected = true;
        bool _singleSelected;
        bool _multipleSelected;
        bool _mixedSelected;

        Vector2 _scrollPosition;

        ReorderableList l_variantKeyTypes;

        private object[] _selectedObjects = new object[0];
        public object[] SelectedObjects
        {
            get => _selectedObjects;
            set
            {
                _selectedObjects = value;

                _noneSelected = _selectedObjects.Length == 0;
                _singleSelected = _selectedObjects.Length == 1;
                _multipleSelected = _selectedObjects.Length > 1;

                _mixedSelected = false;
                _selectedObjectsType = _noneSelected ?
                    null :
                    _selectedObjects.First().GetType();

                foreach (var item in _selectedObjects)
                {
                    if (item.GetType() == _selectedObjectsType) continue;
                    _mixedSelected = true;
                    _selectedObjectsType = null;
                    break;
                }

                HandleSelectionChange();
            }
        }

        private void VariantTree_OnChangeSelection(int[] ids)
        {
            var items = ids
                .Where(x => window.asset.Variants.IndexInRange(x))
                .Select(x => window.asset.Variants[x])
                .ToArray();

            SelectedObjects = items;
        }

        public void Initialize(PromptLibraryWindow window)
        {
            this.window = window;

            window.variantTree.OnChangeSelection += VariantTree_OnChangeSelection;

            l_variantKeyTypes = new ReorderableList(new List<string>(), typeof(string));
            l_variantKeyTypes.drawHeaderCallback += (Rect rect) =>
            {
                GUI.Label(rect, "Key Types");
            };

            l_variantKeyTypes.onChangedCallback += _ =>
            {
                foreach (var item in SelectedObjects)
                {
                    if (!(item is PromptsVariant variant)) continue;
                    variant.keyTypes = ((List<string>)l_variantKeyTypes.list).ToArray();
                }
            };

            l_variantKeyTypes.onAddCallback += _ =>
            {
                l_variantKeyTypes.list.Add(string.Empty);
            };

            l_variantKeyTypes.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                using (new EditorGUI.ChangeCheckScope())
                    l_variantKeyTypes.list[index] = EditorGUI.DelayedTextField(rect, l_variantKeyTypes.list[index] as string);

                foreach (var item in SelectedObjects)
                {
                    if (!(item is PromptsVariant variant)) continue;
                    variant.keyTypes = ((List<string>)l_variantKeyTypes.list).ToArray();
                }
            };

            SelectedObjects = SelectedObjects;
        }

        void HandleSelectionChange()
        {
            l_variantKeyTypes.list.Clear();

            if (_mixedSelected || _noneSelected) return;
            switch (SelectedObjects.First())
            {
                case PromptsVariant variant:
                    l_variantKeyTypes.list = new List<string>(variant.keyTypes);
                    break;
            }
        }

        public void OnGUI()
        {
            using (new EditorChangeChecker.ChangeCheck(window.SetAssetDirty))
            {
                using (var scroll = new GUILayout.ScrollViewScope(_scrollPosition))
                {
                    _scrollPosition = scroll.scrollPosition;

                    if (window.asset == null)
                    {
                        GUILayout.Label("Asset Not Loaded", EditorStyles.centeredGreyMiniLabel);
                        return;
                    }

                    if (_noneSelected)
                    {
                        GUILayout.Label("No Object Selected", EditorStyles.centeredGreyMiniLabel);
                        return;
                    }

                    if (_mixedSelected)
                    {
                        EditorGUILayout.HelpBox("There are multiple objects selected of different type", MessageType.None);
                        return;
                    }

                    switch (SelectedObjects[0])
                    {
                        case PromptsVariant variant:
                            variant.name = MultipleSelectedTextField("Name", variant.name);
                            l_variantKeyTypes.DoLayoutList();
                            break;
                    }

                    EditorGUILayout.Space();
                    GUILayout.FlexibleSpace();
                }
            }
        }

        private string MultipleSelectedTextField(string label, string text)
        {
            string value;
            using (new EditorGUI.DisabledGroupScope(!_singleSelected))
            {
                EditorGUI.showMixedValue = _multipleSelected;
                value = EditorGUILayout.DelayedTextField(label, text);
                EditorGUI.showMixedValue = false;
            }

            return _multipleSelected ?
                text :
                value;
        }
    }
}