using UnityEngine;
using UnityEditor;

using UEditor = UnityEditor.Editor;
using Project.Translation.Defines;

namespace Project.Editor.Translation.Defines
{
    [CustomEditor(typeof(MultiEntryTranslationDefines))]
    internal class MultiEntryTranslationDefinesInspector : UEditor
    {
        protected override void OnHeaderGUI()
        {

        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledGroupScope(true))
                base.OnInspectorGUI();
        }
    }
}