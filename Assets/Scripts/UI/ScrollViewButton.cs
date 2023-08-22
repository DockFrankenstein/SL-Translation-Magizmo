using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Project.UI
{
    public class ScrollViewButton : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public Button button;

        public bool selected;

        [Label("Themes")]
        [BeginGroup("normal")]
        [EndGroup]
        public ColorBlock normalTheme = new ColorBlock();
        [BeginGroup("selected")]
        [EndGroup]
        public ColorBlock selectedTheme = new ColorBlock();

        public event Action<bool> OnSelected;

        private void Reset()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            button = GetComponent<Button>();

            if (button)
            {
                normalTheme = button.colors;
                selectedTheme = button.colors;
            }
        }

        public void ChangeState(bool state)
        {
            ChangeStateSilent(state);
            OnSelected?.Invoke(selected);
        }

        public void ChangeStateSilent(bool state)
        {
            selected = state;
            button.colors = selected ? selectedTheme : normalTheme;
        }

        public void ToggleState()
        {
            ChangeState(!selected);
        }
    }
}