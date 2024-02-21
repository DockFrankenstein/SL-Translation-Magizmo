using UnityEngine;

namespace Project.Cam
{
    public class SetCameraTarget : MonoBehaviour 
    {
        [SerializeField] CameraTarget targetAsset;
        [SerializeField] Camera cam;

        private void Reset()
        {
            cam = GetComponent<Camera>();
        }

        private void Awake()
        {
            if (targetAsset != null && cam != null)
                targetAsset.Cam = cam;
        }
    }
}