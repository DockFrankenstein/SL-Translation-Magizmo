using UnityEngine;

namespace qASIC.Input.KeyProviders
{
    public class KeyCodeKeyProvider : KeyTypeProvider<KeyCode>
    {
        public override string RootPath => "key_keyboard";
        public override string DisplayName => "Keyboard";
    }
}