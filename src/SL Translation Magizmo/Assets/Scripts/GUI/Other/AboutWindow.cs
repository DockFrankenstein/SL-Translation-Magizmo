using qASIC;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Other
{
    public class AboutWindow : MonoBehaviour
    {
        [SerializeField] UIDocument document;

        [Space]
        [Message("{0} - project version\n{1} - Unity version")]
        [TextArea]
        [SerializeField] string instanceDataFormat = "Version <b>{0}</b>, made with <b>Unity {1}</b>";

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        Button _closeButton;
        Label _instanceData;

        private void Awake()
        {
            var root = document.rootVisualElement;
            _instanceData = root.Q<Label>("instance-data");
            _closeButton = root.Q<Button>("close");

            root.ChangeDispaly(false);

            _instanceData.text = string.Format(instanceDataFormat, Application.version, Application.unityVersion);

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
