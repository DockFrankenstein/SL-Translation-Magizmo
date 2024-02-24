using System;
using System.Linq;
using UnityEngine.UIElements;

namespace Project.UI
{
    public class PopupButton : Button
    {
        public new class UxmlFactory : UxmlFactory<PopupButton, UxmlTraits> { }

        public new class UxmlTraits : TextElement.UxmlTraits
        {
            UxmlBoolAttributeDescription _opened = new UxmlBoolAttributeDescription()
            {
                name = "Opened",
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var btn = (PopupButton)ve;

                bool isOpened = btn.Opened;
                if (_opened.TryGetValueFromBag(bag, cc, ref isOpened) && isOpened != btn.Opened)
                    btn.ChangeOpenedState(isOpened);
            }
        }

        public VisualElement Root { get; set; }

        VisualElement Arrow { get; set; }
        VisualElement ContentContainer { get; set; }

        public bool Opened { get; private set; }
        public event Action<bool> OnOpenedOrClosed;

        public override VisualElement contentContainer => ContentContainer;

        public VisualElement Content =>
            Children().FirstOrDefault();

        public PopupButton()
        {
            ContentContainer = new VisualElement();
            ContentContainer.style.position = Position.Absolute;
            ContentContainer.style.left = 0f;
            ContentContainer.style.right = 0f;
            ContentContainer.style.bottom = 0f;
            ContentContainer.style.alignItems = Align.Stretch;
            ContentContainer.style.height = 0f;
            contentContainer.style.marginBottom = 0f;
            contentContainer.style.marginLeft = 0f;
            contentContainer.style.marginRight = 0f;
            contentContainer.style.marginTop = 0f;
            ContentContainer.name = "popup-content-container";
            ContentContainer.AddToClassList("popup-content-container");
            ContentContainer.ChangeDispaly(false);

            Arrow = new VisualElement();
            Arrow.style.width = 25f;
            Arrow.style.height = 25f;
            Arrow.style.position = Position.Absolute;
            Arrow.style.top = new StyleLength(StyleKeyword.Auto);
            Arrow.style.bottom = new StyleLength(StyleKeyword.Auto);
            Arrow.style.right = 4f;
            Arrow.name = "popup-button-arrow";
            Arrow.AddToClassList("popup-button-arrow");

            hierarchy.Add(Arrow);
            hierarchy.Add(ContentContainer);

            style.overflow = Overflow.Visible;
            
            clicked += () =>
            {
                ChangeOpenedState(!Opened);
            };
        }

        public void ChangeOpenedState(bool state)
        {
            Opened = state;
            ContentContainer.ChangeDispaly(Opened);
            OnOpenedOrClosed?.Invoke(state);
        }
    }
}