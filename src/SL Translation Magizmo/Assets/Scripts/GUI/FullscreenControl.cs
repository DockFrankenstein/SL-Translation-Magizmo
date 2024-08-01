using Project.Translation;
using qASIC.Input;
using qASIC.Options;
using UnityEngine;

namespace Project.GUI
{
    public class FullscreenControl : MonoBehaviour
    {
        [Header("Assign")]
        [SerializeField] TranslationManager manager;

        [Header("Input")]
        public InputMapItemReference i_toggle;


        [Option("fullscreen", false)] private static bool Sett_Fullscreen { get; set; }
        [Option("maximized", true)] private static bool Sett_Maximized { get; set; }
        [Option("resolution_x")] private static int Sett_ResolutionX { get; set; }
        [Option("resolution_y")] private static int Sett_ResolutionY { get; set; }
        [Option("use_default_fullscreen_behaviour")] private static bool Sett_DefaultFullscreen { get; set; }

        public bool IsFullscreen 
        {
            get => Sett_Fullscreen;
            set
            {
                manager.Options.SetOptionAndApply("fullscreen", value);
            }
        }

        private void Awake()
        {
            UpdateFullscreen();
        }

        private void OnEnable()
        {
            manager.Options.OptionsList["fullscreen"].OnValueChanged += _ => UpdateFullscreen();
            manager.Options.OptionsList["maximized"].OnValueChanged += _ => UpdateFullscreen();
        }

        private void OnDisable()
        {
            manager.Options.OptionsList["fullscreen"].OnValueChanged -= _ => UpdateFullscreen();
            manager.Options.OptionsList["maximized"].OnValueChanged -= _ => UpdateFullscreen();
        }

        private void Update()
        {
            var mode = Screen.fullScreenMode;
            bool maximizedThisFrame = mode == FullScreenMode.MaximizedWindow && Sett_DefaultFullscreen;
            maximizedThisFrame |= Screen.width == Screen.currentResolution.width && !Sett_DefaultFullscreen;
            maximizedThisFrame |= Screen.height == Screen.currentResolution.height && !Sett_DefaultFullscreen;

            if (!Sett_Fullscreen && Sett_Maximized == !maximizedThisFrame)
                manager.Options.SetOptionAndApply("maximized", maximizedThisFrame);

            if (!Sett_Maximized && !Sett_Fullscreen && Screen.width != Sett_ResolutionX)
                manager.Options.SetOptionAndApply("resolution_x", Screen.width);

            if (!Sett_Maximized && !Sett_Fullscreen && Screen.height != Sett_ResolutionY)
                manager.Options.SetOptionAndApply("resolution_y", Screen.height);


            if (i_toggle.GetInputDown())
                ToggleFullscreen();
        }

        public void ToggleFullscreen() =>
            IsFullscreen = !IsFullscreen;

        public static void UpdateFullscreen()
        {
            if (Sett_ResolutionX < 100)
                Sett_ResolutionX = 100;

            if (Sett_ResolutionY < 100)
                Sett_ResolutionY = 100;

            var width = Sett_Fullscreen ? Screen.mainWindowDisplayInfo.width : Sett_ResolutionX;
            var height = Sett_Fullscreen ? Screen.mainWindowDisplayInfo.height : Sett_ResolutionY;
            var mode = Sett_Fullscreen ? FullScreenMode.FullScreenWindow : (Sett_Maximized ? FullScreenMode.MaximizedWindow : FullScreenMode.Windowed);

            Screen.SetResolution(width, height, mode);
        }
    }
}