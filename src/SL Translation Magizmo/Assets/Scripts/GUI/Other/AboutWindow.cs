using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Other
{
    public class AboutWindow : MonoBehaviour
    {
        [SerializeField] UIDocument document;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        Button _closeButton;

        private void Awake()
        {
            var root = document.rootVisualElement;
            _closeButton = root.Q<Button>("close");

            root.ChangeDispaly(false);

            _closeButton.clicked += () =>
            {
                root.ChangeDispaly(false);
            };
        }

        public void Open()
        {
            document.rootVisualElement.ChangeDispaly(true);
        }
    }
}
