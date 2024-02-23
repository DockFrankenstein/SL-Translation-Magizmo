using Project.GUI.Preview;
using UnityEditor;
using UnityEngine;
using qASIC;
using qASIC.EditorTools;

namespace Project.GUI.Editor.Preview
{
    [CustomPropertyDrawer(typeof(MappedIdTarget))]
    public class MappedIdTargetDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 6f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var singleLine = EditorGUIUtility.singleLineHeight;
            var separator = EditorGUIUtility.standardVerticalSpacing;

            var backgroundRect = position;
            var labelBackgroundRect = position.ResizeToTop(singleLine);

            var labelRect = labelBackgroundRect.Border(separator, 0f);

            var idRect = labelRect.NextLine();
            var isMainRect = idRect.NextLine();
            var defaultValueRect = isMainRect.NextLine().SetHeight(singleLine * 3 + separator * 2);

            if (Event.current.type == EventType.Repaint)
            {
                new GUIStyle().WithBackgroundColor(qGUIEditorUtility.ButtonColor).Draw(backgroundRect, GUIContent.none, false, false, false, false);
                new GUIStyle().WithBackgroundColor(qGUIEditorUtility.BorderColor).Draw(labelBackgroundRect, GUIContent.none, false, false, false, false);
                qGUIEditorUtility.BorderAround(backgroundRect);
            }

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.PropertyField(idRect, property.FindPropertyRelative("entryId"));
            EditorGUI.PropertyField(isMainRect, property.FindPropertyRelative("isMain"));
            EditorGUI.PropertyField(defaultValueRect, property.FindPropertyRelative("defaultValue"));
        }
    }
}
