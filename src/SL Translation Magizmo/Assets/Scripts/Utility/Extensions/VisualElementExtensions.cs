using UnityEngine.UIElements;

namespace Project
{
    public static class VisualElementExtensions
    {
        public static void ChangeDispaly(this VisualElement element, bool display) =>
            element.style.display = display ?
                DisplayStyle.Flex :
                DisplayStyle.None;
    }
}