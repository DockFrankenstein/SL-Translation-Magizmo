using qASIC.Input.Devices;
using System;
using UnityEngine;
using System.Linq;
using qASIC.Input.Prompts;

namespace qASIC.Input.Map
{
    [Serializable]
    public class Input3DAxis : InputMapItem<Vector3>, ISupportsPrompts
    {
        public Input3DAxis() : base() { }
        public Input3DAxis(string name) : base(name) { }

        public Axis XAxis = new Axis();
        public Axis YAxis = new Axis();
        public Axis ZAxis = new Axis();

        public MapItemPromptData GetPromptData() =>
            new MapItemPromptData(map, XAxis.positiveGuid, XAxis.negativeGuid,
                YAxis.positiveGuid, YAxis.negativeGuid,
                ZAxis.positiveGuid, ZAxis.negativeGuid);

        public string KeysToPromptText(string[] keys) =>
            keys.Where(x => x.Length > 1).Count() == 0 ?
                $"{keys[4]}{keys[0]}{keys[3]}{keys[1]}{keys[2]}{keys[5]}" :
                $"{keys[4]}, {keys[0]}, {keys[3]}, {keys[1]}, {keys[2]}, {keys[5]}";

        public override Vector3 ReadValue(InputMapData data, IInputDevice device) =>
            new Vector3(XAxis.ReadValue(map, data, device), YAxis.ReadValue(map, data, device), ZAxis.ReadValue(map, data, device));

        public override InputEventType GetInputEvent(InputMapData data, IInputDevice device) =>
            XAxis.GetInputEvent(map, data, device) |
            YAxis.GetInputEvent(map, data, device);

        public override Vector3 GetHighestValue(Vector3 a, Vector3 b) =>
            a.magnitude > b.magnitude ? a : b;

        public override bool HasErrors() =>
            XAxis.HasErrors(map) ||
            YAxis.HasErrors(map) ||
            ZAxis.HasErrors(map);
    }
}