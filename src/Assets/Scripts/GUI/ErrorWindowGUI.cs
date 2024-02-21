using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI
{
    public class ErrorWindowGUI : MonoBehaviour
    {
        [SerializeField] UIDocument document;
        [SerializeField] ErrorWindow errorWindow;
        [EditorButton(nameof(DebugOpen))]
        [SerializeField] float closeTimeLength = 0.3f;

        VisualElement container;
        VisualElement fade;
        VisualElement window;
        Label header;
        Label content;
        Button okayButton;

        public bool IsOpened { get; private set; }

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        private void Awake()
        {
            var root = document.rootVisualElement;

            container = root.Q("container");
            fade = root.Q("fade");
            window = root.Q("window");
            header = root.Q<Label>("header");
            content = root.Q<Label>("content");
            okayButton = root.Q<Button>("ok-button");

            okayButton.clicked += Close;

            fade.style.opacity = 0f;
            window.style.opacity = 0f;

            errorWindow.OnPromptCreated += _ => OpenFromErrorWindow();
        }

        //Hide the entire document after all closing animations stop
        //To prevent blocking input
        float _t = 0f;
        bool _waitForClose = false;
        private void Update()
        {
            if (!_waitForClose) return;

            _t += Time.unscaledDeltaTime;
            if (_t >= closeTimeLength)
            {
                container.style.display = DisplayStyle.None;
                _t = 0f;
                _waitForClose = false;
            }
        }

        void DebugOpen()
        {
            if (Application.isPlaying)
                Open("Test", "This is a test");
        }

        public void OpenFromErrorWindow()
        {
            if (IsOpened) return;
            if (errorWindow.PromptsQueue.Count == 0) return;
            var prompt = errorWindow.PromptsQueue.Dequeue();
            Open(prompt.header ?? errorWindow.defaultHeader, prompt.content ?? errorWindow.defaultContent);
        }

        public void Open(string headerText, string contentText)
        {
            //Close if already opened
            if (IsOpened)
                Close();

            _waitForClose = false;
            _t = 0f;

            header.text = headerText;
            content.text = contentText;

            IsOpened = true;
            container.style.display = DisplayStyle.Flex;
            fade.style.opacity = 1f;
            window.style.opacity = 1f;
        }

        public void Close()
        {
            //Don't close if it's already closed
            if (!IsOpened) return;

            fade.style.opacity = 0f;
            window.style.opacity = 0f;

            IsOpened = false;

            _waitForClose = true;
            _t = 0f;

            OpenFromErrorWindow();
        }
    }
}