using Project.GUI.Preview;
using UnityEditor;
using UnityEngine;
using qASIC;
using qASIC.EditorTools;
using Project.Editor;
using System;

namespace Project.GUI.Editor.Preview
{
    [CustomPropertyDrawer(typeof(MappedIdTarget))]
    public class MappedIdTargetDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUIUtility.singleLineHeight * 8f + EditorGUIUtility.standardVerticalSpacing * 9f - 2f + contextHeight;

        float contextHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var singleLine = EditorGUIUtility.singleLineHeight;
            var separator = EditorGUIUtility.standardVerticalSpacing;

            var contentProperty = property.FindPropertyRelative("content");
            var contentValue = contentProperty.objectReferenceValue as MappedIdContent;

            var contentContextProperty = property.FindPropertyRelative("contentContext");
            contentContextProperty.isExpanded = true;
            var contentContextValue = contentContextProperty.managedReferenceValue;

            contextHeight = contentContextValue != null ?
                EditorGUI.GetPropertyHeight(contentContextProperty, includeChildren: true) - singleLine :
                0f;

            var backgroundRect = position;
            var labelBackgroundRect = position.ResizeToTop(singleLine);

            var labelRect = labelBackgroundRect.Border(separator, 0f);

            var idRect = labelRect.NextLine();
            var isMainRect = idRect.NextLine();
            var contentLabelRect = isMainRect.NextLine();
            var lineRect = contentLabelRect.ResizeToTop(0f).Border(-2f, 0f);
            var defaultValueRect = contentLabelRect.NextLine().SetHeight(singleLine * 3 + separator * 2).MoveY(-2f);
            var contentRect = defaultValueRect.NextLine().SetHeight(singleLine);
            var contentContextRect = contentRect.NextLine().BorderLeft(singleLine).SetHeight(contextHeight);

            if (contentValue?.ContextType != contentContextValue?.GetType())
            {
                var val = contentValue?.ContextType == null ?
                    null :
                    Activator.CreateInstance(contentValue.ContextType);

                contentContextProperty.managedReferenceValue = val;
            }

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
            EditorGUI.PropertyField(defaultValueRect, property.FindPropertyRelative("defaultValue"));
            EditorGUI.PropertyField(contentRect, contentProperty);

            if (contentContextValue != null)
                qGUIEditorUtility.DrawProperty(contentContextRect, contentContextProperty);
        }
    }
}
