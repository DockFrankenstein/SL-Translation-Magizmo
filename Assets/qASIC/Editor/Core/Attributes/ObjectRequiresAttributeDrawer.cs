#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;

namespace qASIC.Internal
{
    [CustomPropertyDrawer(typeof(ObjectRequiresAttribute))]
    public class ObjectRequiresAttributeDrawer : PropertyDrawer
    {
        ObjectRequiresAttribute attr;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            attr = (ObjectRequiresAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label, new GUIContent("Use [ObjectRequires] with object references."));
                return;
            }


            object obj = fieldInfo.GetValue(property.serializedObject.targetObject);
            bool setValue = false;
            switch (obj)
            {
                case IEnumerable<object> array:
                    EditorGUI.LabelField(position, label, new GUIContent("Arrays and Lists aren't supported."));
                    break;
                default:
                    EditorGUI.PropertyField(position, property);

                    //idk, but this only works
                    //I hate this
                    if (obj?.Equals(null) != false)
                        return;

                    List<Type> remainingTypes = GetRemainingComponentTypes(obj);
                    if (remainingTypes.Count != 0)
                    {
                        fieldInfo.SetValue(property.serializedObject.targetObject, null);
                        LogWrongProperty();
                        obj = null;
                        setValue = true;
                    }

                    break;
            }

            if (setValue)
                fieldInfo.SetValue(property.serializedObject.targetObject, obj);
        }

        void LogWrongProperty() =>
            Debug.LogError($"Property requires {string.Join<Type>(", ", attr.RequiredTypes)} {(attr.RequiredTypes.Length > 1 ? "components" : "component")}");

        List<Type> GetRemainingComponentTypes(object obj)
        {
            Type[] types = attr.RequiredTypes;

            List<Type> remainingTypes = new List<Type>();
            switch (obj)
            {
                case GameObject gameObj:
                    foreach (Type type in types)
                        if (gameObj.GetComponent(type) == null)
                            remainingTypes.Add(type);
                    break;
                default:
                    Type objType = obj.GetType();
                    foreach (Type type in types)
                        if (!type.IsAssignableFrom(objType) && objType != type)
                            remainingTypes.Add(type);
                    break;
            }

            return remainingTypes;
        }
    }
}
#endif