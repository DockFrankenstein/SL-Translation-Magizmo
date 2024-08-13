using UnityEngine;
using UnityEngine.UIElements;
using Project.Translation;
using System.Linq;
using qASIC;

using Fab.UITKDropdown;
using qASIC.Input;
using qASIC.Input.Prompts;
using qASIC.Input.Players;
using System;
using qASIC.Input.Map;

namespace Project.GUI.Top
{
    public abstract class TopMenu : MonoBehaviour
    {
        [BeginGroup("UI")]
        public UIDocument document;
        [EndGroup]
        public TranslationManager manager;

        protected Button button;

        protected DropdownMenu menu;
        protected Dropdown dropdown;

        protected abstract string ButtonName { get; }

        protected virtual void Reset()
        {
            document = GetComponent<UIDocument>();

            manager = GetComponents<TopMenu>()
                .Select(x => x.manager)
                .Where(x => x != null)
                .FirstOrDefault();

        }

        protected virtual void Awake()
        {
            var root = document.rootVisualElement;

            button = root.Q<Button>(ButtonName);
            button.clicked += Button_clicked;
            menu = new DropdownMenu();
            dropdown = new Dropdown(root, MakeCustomItem, SetCustomItem);
        }

        protected abstract void CreateMenu();

        private void Button_clicked()
        {
            menu.ClearItems();
            CreateMenu();
            dropdown.Open(menu, button.worldBound);
        }

        private VisualElement MakeCustomItem()
        {
            var ve = new VisualElement();

            ve.Add(new VisualElement().WithClass(Dropdown.itemIconClassname).WithName("icon"));

            ve.Add(new Label()
                .WithClass(Dropdown.itemTextClassname)
                .WithName("text"));

            var hotkeyText = new Label();
            hotkeyText
                .WithClass(Dropdown.itemTextClassname)
                .WithName("hotkey-text");
            hotkeyText.style.unityTextAlign = TextAnchor.MiddleRight;
            hotkeyText.style.marginLeft = 24f;
            ve.Add(hotkeyText);

            ve.Add(new VisualElement()
                .WithClass(Dropdown.itemArrowClassname)
                .WithName("arrow"));

            return ve;
        }

        private void SetCustomItem(VisualElement ve, DropdownMenuItem item, string[] path, int level)
        {
            ve.Q<Label>("text").text = path[level];

            if (path.Length - 1 == level &&
                item is DropdownMenuAction actionItem && actionItem.userData is ItemData data)
            {
                ve.Q<Label>(name: "hotkey-text").text = data.hotkey;
            }
            else
            {
                ve.Q<Label>(name: "hotkey-text").text = string.Empty;
            }
        }

        protected ItemData CreateDataForInput(PromptLibrary library, InputMapItemReference reference)
        {
            var item = reference.GetItem();

            if (!(item is ISupportsPrompts promptItem))
                return new ItemData();

            var promptData = promptItem.GetPromptData();

            if (promptData.promptGroups.Count == 0)
                return new ItemData();

            var txt = library
                .ForDevice(InputPlayerManager.Players[0].LastDevice ?? InputPlayerManager.Players[0].CurrentDevice)
                .GetPromptsFromPaths(promptData.promptGroups[0].keyPaths)
                .ToDisplayNames();

            return new ItemData(string.Join("+", txt));
        }

        public class ItemData
        {
            public ItemData() { }

            public ItemData(string hotkey)
            {
                this.hotkey = hotkey;
            }

            public string hotkey;
        }
    }
}