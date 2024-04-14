using UnityEditor;
using UnityEngine;

namespace Project.Utility.Editor
{
    [CustomPropertyDrawer(typeof(Version))]
    public class VersionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var releasesProperty = property.FindPropertyRelative("releases");

            int[] releases = new int[releasesProperty.arraySize];
            for (int i = 0; i < releasesProperty.arraySize; i++)
                releases[i] = releasesProperty.GetArrayElementAtIndex(i).intValue;

            position = EditorGUI.PrefixLabel(position, label);

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                var showMixedPrev = EditorGUI.showMixedValue;
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

                var text = EditorGUI.TextField(position, string.Join(".", releases));

                EditorGUI.showMixedValue = showMixedPrev;

                if (change.changed && Version.TryParse(text, out var newVersion))
                {
                    releasesProperty.arraySize = newVersion.releases.Length;
                    for (int i = 0; i < releasesProperty.arraySize; i++)
                        releasesProperty.GetArrayElementAtIndex(i).intValue = (int)newVersion.releases[i];
                }
            }

            if (property.serializedObject.hasModifiedProperties)
                property.serializedObject.ApplyModifiedProperties();
        }
    }
}
