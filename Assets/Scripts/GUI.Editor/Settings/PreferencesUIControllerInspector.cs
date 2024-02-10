using UnityEditor;
using UnityEngine;
using qASIC;

namespace Project.GUI.Settings.Editor
{
    [CustomPropertyDrawer(typeof(UIDocumentSettings.Target))]
    public class PreferencesUITargetDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelRect = position.SetHeight(EditorGUIUtility.singleLineHeight);
            var elementNameRect = labelRect.NextLine();
            var targetOptionRect = elementNameRect.NextLine();

            var targetType = (property.managedReferenceValue as UIDocumentSettings.Target)?.ValueType;

            EditorGUI.LabelField(labelRect, new GUIContent($"Value Type: {targetType?.Name ?? "NULL"}"), EditorStyles.boldLabel);

            if (property.managedReferenceValue != null)
            {
                EditorGUI.PropertyField(elementNameRect, property.FindPropertyRelative("elementName"));
                EditorGUI.PropertyField(targetOptionRect, property.FindPropertyRelative("targetOption"));
            }
        }
    }
}