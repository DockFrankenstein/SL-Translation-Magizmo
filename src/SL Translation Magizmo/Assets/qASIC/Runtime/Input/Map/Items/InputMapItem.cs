﻿using UnityEngine;
using System;
using qASIC.Input.Devices;

namespace qASIC.Input.Map
{
    [Serializable]
    public abstract class InputMapItem : INonRepeatable, IMapItem
    {
        public InputMapItem() { }

        public InputMapItem(string name)
        {
            itemName = name;
        }

        [SerializeField] string itemName;
        [SerializeField] string guid = System.Guid.NewGuid().ToString();

        [NonSerialized] internal InputMap map;

        /// <summary>Name of the item</summary>
        public string ItemName { get => itemName; set => itemName = value; }
        public string Guid { get => guid; set => guid = value; }
        public bool MapLoaded => map != null;

        public virtual Color ItemColor => new Color(0f, 0.7019607843137255f, 1f);


        public abstract Type ValueType { get; }

        public override string ToString() =>
            itemName;

        public bool NameEquals(string name) =>
            NonRepeatableChecker.Compare(itemName, name);

        public virtual void OnCreated() { }

        public abstract object ReadValueAsObject(InputMapData data, IInputDevice device);
        public abstract InputEventType GetInputEvent(InputMapData data, IInputDevice device);
        public abstract object GetHighestValueAsObject(object a, object b);

        /// <summary>Checks if the item has any errors. This is used in the editor to signify if any changes are needed.</summary>
        public virtual bool HasErrors() =>
            false;
    }

    [Serializable]
    public abstract class InputMapItem<T> : InputMapItem
    {
        public InputMapItem() : base() { }
        public InputMapItem(string name) : base(name) { }

        public override Type ValueType => typeof(T);

        public override object ReadValueAsObject(InputMapData data, IInputDevice device) =>
            ReadValue(data, device);

        public abstract T ReadValue(InputMapData data, IInputDevice device);


        public override object GetHighestValueAsObject(object a, object b) =>
            GetHighestValue((T)a, (T)b);
        public abstract T GetHighestValue(T a, T b);
    }
}