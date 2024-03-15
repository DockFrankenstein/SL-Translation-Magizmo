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
            EditorGUIUtility.singleLineHeight * 8f + EditorGUIUtility.standardVerticalSpacing * 8f - 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var singleLine = EditorGUIUtility.singleLineHeight;
            var separator = EditorGUIUtility.standardVerticalSpacing;

            var backgroundRect = position;
            var labelBackgroundRect = position.ResizeToTop(singleLine);

            var labelRect = labelBackgroundRect.Border(separator, 0f);

            var idRect = labelRect.NextLine();
            var isMainRect = idRect.NextLine();
            var contentLabelRect = isMainRect.NextLine();
            var contentRect = contentLabelRect.NextLine().MoveY(-2f);
            var defaultValueRect = contentRect.NextLine().SetHeight(singleLine * 3 + separator * 2);

            var lineRect = contentLabelRect.ResizeToTop(0f).Border(-2f, 0f);

            if (Event.current.type == EventType.Repaint)
            {
                new GUIStyle().WithBackgroundColor(qGUIEditorUtility.ButtonColor).Draw(backgroundRect, GUIContent.none, false, false, false, false);
                new GUIStyle().WithBackgroundColor(qGUIEditorUtility.BorderColor).Draw(labelBackgroundRect, GUIContent.none, false, false, false, false);
                qGUIEditorUtility.BorderAround(backgroundRect);
            }

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.PropertyField(idRect, property.FindPropertyRelative("entryId"));
            EditorGUI.PropertyField(isMainRect, property.FindPropertyRelative("isMain"));
            qGUIEditorUtility.HorizontalLine(lineRect);
            EditorGUI.LabelField(contentLabelRect, "Content", EditorStyles.boldLabel);
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("content"));
            EditorGUI.PropertyField(defaultValueRect, property.FindPropertyRelative("defaultValue"));
        }
    }
}
