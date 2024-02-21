using qASIC.Internal;
using System;
using System.Collections.Generic;

namespace qASIC.Input.Devices
{
    public interface IInputDevice
    {
        string DeviceName { get; set; }
        string DeviceType { get; }

        /// <summary>List of all root paths the device uses</summary>
        string[] KeyRoots { get; }

        /// <summary>Can this device be used in the editor while the game isn't running?</summary>
        bool RuntimeOnly { get; }

        float GetInputValue(string keyPath);
        InputEventType GetInputEvent(string keyPath);
        string GetAnyKeyDown();
        Dictionary<string, float> Values { get; }

        void Initialize();
        void Update();

        /// <summary>This method is used for displaying information in the device inspector</summary>
        PropertiesList GetProperties();
    }
}