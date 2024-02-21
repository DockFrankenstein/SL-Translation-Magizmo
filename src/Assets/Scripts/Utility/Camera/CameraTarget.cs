using UnityEngine;

namespace Project.Cam
{
    [CreateAssetMenu(fileName = "New Camera Target", menuName = "Scriptable Objects/Camera/Target")]
    public class CameraTarget : ScriptableObject
    {
        public Camera Cam { get; set; }
    }
}