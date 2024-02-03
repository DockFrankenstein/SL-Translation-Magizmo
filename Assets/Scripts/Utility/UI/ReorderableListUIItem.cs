using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Project.Utility.UI
{
    public class ReorderableListUIItem : MonoBehaviour
    {
        public TMP_InputField input;
        public Button upButton;
        public Button downButton;

        [HideInInspector] public ReorderableListUI reorderableList;

        private void Awake()
        {
            if (input != null)
                input.onValueChanged.AddListener(Input_OnValueChanged);
        }

        private void Input_OnValueChanged(string value)
        {
            if (reorderableList == null) return;
            reorderableList.UpdateItem(this);
        }

        public void MoveItem(int amount)
        {
            if (reorderableList == null) return;
            reorderableList.MoveItem(this, amount);
        }

        public void RemoveItem()
        {
            if (reorderableList == null) return;
            reorderableList.DeleteItem(this);
        }
    }
}