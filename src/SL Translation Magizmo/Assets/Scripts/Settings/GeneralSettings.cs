using qASIC.Options;
using System.IO;

namespace Project.Settings
{
    public static class GeneralSettings
    {
        public const string DEFAULT_TRANSLATION_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\SCP Secret Laboratory\Translations";

        private static string _translationPath = null;
        [Option("translation_path")]
        public static string TranslationPath 
        {
            get
            {
                if (_translationPath == null)
                    _translationPath = Directory.Exists(DEFAULT_TRANSLATION_PATH) ?
                    DEFAULT_TRANSLATION_PATH :
                    string.Empty;

                return _translationPath;
            }
            set => _translationPath = value;
        }
    }
}