namespace qASIC.Input.KeyProviders
{
    public class GamepadButtonKeyProvider : KeyTypeProvider<GamepadButton>
    {
        public override string RootPath => "key_gamepad";
        public override string DisplayName => "Gamepad";
    }
}