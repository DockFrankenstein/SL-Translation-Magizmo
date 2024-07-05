using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using qASIC.Options;
using System;

namespace Project.GUI
{
    public class GUIController : MonoBehaviour
    {
        [SerializeField] CanvasScaler canvasScaler;
        [SerializeField] PanelSettings panelSettings;

        private static float _scaleFactor = 1f;
        [Option("ui_scale_factor")]
        public static float ScaleFactor 
        {
            get => _scaleFactor;
            set
            {
                _scaleFactor = value;
                OnChangeScale?.Invoke();
            }
        }

        static event Action OnChangeScale;

        private void Awake()
        {
            UpdateScale();
        }

        private void OnEnable()
        {
            OnChangeScale += UpdateScale;
        }

        private void OnDisable()
        {
            OnChangeScale -= UpdateScale;
        }

        void UpdateScale()
        {
            canvasScaler.scaleFactor = ScaleFactor;
            panelSettings.scale = ScaleFactor;
        }

        private void OnDestroy()
        {
            panelSettings.scale = 1f;
        }
    }
}
