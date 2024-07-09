using UnityEngine.UIElements;

namespace Project
{
    public static class VisualElementExtensions
    {
        public static void ChangeDispaly(this VisualElement element, bool display) =>
            element.style.display = display ?
                DisplayStyle.Flex :
                DisplayStyle.None;

        public static T WithMargin<T>(this T element, float margin) where T : VisualElement
        {
            element.style.marginBottom = margin;
            element.style.marginLeft = margin;
            element.style.marginRight = margin;
            element.style.marginTop = margin;
            return element;
        }
    }
}