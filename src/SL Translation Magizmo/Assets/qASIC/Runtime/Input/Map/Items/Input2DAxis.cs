using qASIC.Input.Devices;
using qASIC.Input.Prompts;
using System;
using UnityEngine;
using System.Linq;

namespace qASIC.Input.Map
{
    [Serializable]
    public class Input2DAxis : InputMapItem<Vector2>, ISupportsPrompts
    {
        public Input2DAxis() : base() { }
        public Input2DAxis(string name) : base(name) { }

        public Axis XAxis = new Axis();
        public Axis YAxis = new Axis();

        public MapItemPromptData GetPromptData() =>
            new MapItemPromptData(map, XAxis.positiveGuid, XAxis.negativeGuid,
                YAxis.positiveGuid, YAxis.negativeGuid);

        public string KeysToPromptText(string[] keys) =>
            keys.Where(x => x.Length > 1).Count() == 0 ?
                $"{keys[0]}{keys[3]}{keys[1]}{keys[2]}" :
                $"{keys[0]}, {keys[3]}, {keys[1]}, {keys[2]}";

        public override Vector2 ReadValue(InputMapData data, IInputDevice device) =>
            new Vector2(XAxis.ReadValue(map, data, device), YAxis.ReadValue(map, data, device));

        public override InputEventType GetInputEvent(InputMapData data, IInputDevice device) =>
            XAxis.GetInputEvent(map, data, device) |
            YAxis.GetInputEvent(map, data, device);

        public override Vector2 GetHighestValue(Vector2 a, Vector2 b) =>
            a.magnitude > b.magnitude ? a : b;

        public override bool HasErrors() =>
            XAxis.HasErrors(map) ||
            YAxis.HasErrors(map);
    }
}