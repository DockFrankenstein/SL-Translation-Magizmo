using qASIC.SettingsSystem;
using System.IO;

namespace Project.Settings
{
    public static class GeneralSettings
    {
        public const string DEFAULT_TRANSLATION_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\SCP Secret Laboratory\Translations";

        public static string TranslationPath { get; set; } = GetDefaultTranslationPath();

        [OptionsSetting("translation_path", defaultValueMethodName = nameof(GetDefaultTranslationPath))]
        static void HandleTranslationPath(string newValue)
        {
            TranslationPath = newValue;
        }

        public static string GetDefaultTranslationPath() =>
            Directory.Exists(DEFAULT_TRANSLATION_PATH) ?
            DEFAULT_TRANSLATION_PATH :
            string.Empty;
    }
}