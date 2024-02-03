using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Project.GUI.Preview
{
    public class SceneDropdownItem : MonoBehaviour
    {
        public TMP_Text text;
        public Button button;

        [HideInInspector] public string path;
    }
}
