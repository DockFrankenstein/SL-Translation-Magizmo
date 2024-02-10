using UnityEngine;
using UnityEngine.UIElements;
using Project.Translation;
using System.Linq;
using qASIC;

using Fab.UITKDropdown;

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

        private void Reset()
        {
            document = GetComponent<UIDocument>();

            manager = GetComponents<TopMenu>()
                .Select(x => x.manager)
                .Where(x => x != null)
                .FirstOrDefault();

        }

        private void Awake()
        {
            var root = document.rootVisualElement;

            button = root.Q<Button>(ButtonName);
            button.clicked += Button_clicked;
            menu = new DropdownMenu();
            dropdown = new Dropdown(root);

            CreateMenu();
        }

        protected abstract void CreateMenu();

        private void Button_clicked()
        {
            //Fix for dropdown not repsecting padding and margin
            var rect = new Rect(button.contentRect)
                .MoveY(8f)
                .MoveX(-8f);

            dropdown.Open(menu, rect);
        }
    }
}