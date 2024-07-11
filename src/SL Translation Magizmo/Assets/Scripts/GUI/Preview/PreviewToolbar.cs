using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Preview
{
    public class PreviewToolbar : MonoBehaviour
    {
        [SerializeField] UIDocument document;
        [SerializeField] TranslationPreviewCamera cam;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        Button _reset;

        private void Awake()
        {
            var root = document.rootVisualElement;

            _reset = root.Q<Button>("preview-reset");

            _reset.clicked += () =>
            {
                cam.ResetPosition();
            };
        }
    }
}