using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using qASIC.SettingsSystem;
using System;

namespace Project.GUI
{
    public class GUIController : MonoBehaviour
    {
        [SerializeField] CanvasScaler canvasScaler;
        [SerializeField] PanelSettings panelSettings;

        public static float ScaleFactor { get; set; } = 1f;
        [OptionsSetting("ui_scale_factor", 1f)]
        static void SettM_ScaleFactor(float value)
        {
            ScaleFactor = value;
            OnChangeScale?.Invoke();
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
