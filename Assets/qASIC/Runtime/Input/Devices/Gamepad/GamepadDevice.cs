using System;

namespace qASIC.Input.Devices
{
    public abstract class GamepadDevice : InputDevice<GamepadButton>, IGamepadDevice
    {
        public override string[] KeyRoots => new string[] { "key_keyboard" };
    }
}