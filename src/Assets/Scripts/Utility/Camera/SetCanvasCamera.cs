using UnityEngine;

namespace Project.Cam
{
    public class SetCanvasCamera : MonoBehaviour
    {
        [SerializeField] Canvas canvas;
        [SerializeField] CameraTarget target;

        private void Reset()
        {
            canvas = GetComponent<Canvas>();
        }

        private void Awake()
        {
            canvas.worldCamera = target?.Cam;
        }
    }
}