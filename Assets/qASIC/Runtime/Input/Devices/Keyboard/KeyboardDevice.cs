using System;
using UnityEngine;

namespace qASIC.Input.Devices
{
    public abstract class KeyboardDevice : InputDevice<KeyCode>, IKeyboardDevice
    {
        public override string DeviceType => "Keyboard";
        public override string[] KeyRoots => new string[] { "key_keyboard" };
    }
}
