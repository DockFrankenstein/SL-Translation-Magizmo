using qASIC;
using UnityEditor;
using UnityEngine;
using System;

namespace Project.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(GUIDAttribute))]
    public class GUIDAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
                EditorGUI.PrefixLabel(position, new GUIContent("Use GUID attribute with string fields only"));

            Rect fieldPosition = new Rect(position).BorderRight(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            Rect buttonPosition = new Rect(position).ResizeToRight(EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(fieldPosition, property, label);

            GUIStyle style = new GUIStyle(EditorStyles.miniButton);
            style.padding = new RectOffset(0, 0, 0, 0);

            if (GUI.Button(buttonPosition, EditorGUIUtility.IconContent("d_Refresh"), style))
            {
                property.stringValue = Guid.NewGuid().ToString();
            }
        }
    }
}