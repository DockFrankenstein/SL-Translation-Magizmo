using TMPro;
using UnityEngine;

namespace Project.GUI.Preview
{
    public class PreviewText : PreviewElement
    {
        [SerializeField] TMP_Text text;
        [TextArea] public string[] contents;

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        protected override void Awake()
        {
            base.Awake();

            if (text != null && contents.Length > 0)
                text.text = contents[0];
        }

        public override void ChangeIndex(int newIndex, bool silent = false)
        {
            base.ChangeIndex(newIndex, silent);
            text.text = contents[Mathf.Clamp(newIndex, 0, contents.Length - 1)];
        }
    }
}