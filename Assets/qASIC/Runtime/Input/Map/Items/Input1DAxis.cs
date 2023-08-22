using qASIC.Input.Devices;
using qASIC.Input.Prompts;
using System;
using UnityEngine;
using System.Linq;

namespace qASIC.Input.Map
{
    [Serializable]
    public class Input1DAxis : InputMapItem<float>, ISupportsPrompts
    {
        public Input1DAxis() : base() { }
        public Input1DAxis(string name) : base(name) { }

        public string positiveGuid = string.Empty;
        public string negativeGuid = string.Empty;

        public MapItemPromptData GetPromptData() =>
            new MapItemPromptData(map, positiveGuid, negativeGuid);

        public string KeysToPromptText(string[] keys) =>
            keys.Where(x => x.Length > 1).Count() == 0 ?
                $"{keys[0]}{keys[1]}" :
                $"{keys[0]}, {keys[1]}";

        public override float ReadValue(InputMapData data, IInputDevice device) =>
            new Axis(positiveGuid, negativeGuid).ReadValue(map, data, device);

        public override InputEventType GetInputEvent(InputMapData data, IInputDevice device) =>
            new Axis(positiveGuid, negativeGuid).GetInputEvent(map, data, device);

        public override float GetHighestValue(float a, float b) =>
            Mathf.Abs(a) > Mathf.Abs(b) ? a : b;

        public override bool HasErrors() =>
            InputMapUtility.IsGuidBroken<InputBinding>(map, positiveGuid) ||
            InputMapUtility.IsGuidBroken<InputBinding>(map, negativeGuid);
    }
}