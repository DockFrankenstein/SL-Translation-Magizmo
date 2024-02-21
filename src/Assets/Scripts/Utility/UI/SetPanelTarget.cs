using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class SetPanelTarget : MonoBehaviour
    {
        [SerializeField] PreviewPanelTarget target;
        [SerializeField] RawImage panel;

        private void Awake()
        {
            target.Image = panel;
        }
    }
}