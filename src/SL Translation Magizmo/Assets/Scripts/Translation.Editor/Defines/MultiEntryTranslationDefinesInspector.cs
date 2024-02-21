using UnityEngine;
using UnityEditor;

using UEditor = UnityEditor.Editor;
using Project.Translation.Mapping;

namespace Project.Editor.Translation.Defines
{
    [CustomEditor(typeof(MultiEntryTranslationMapping))]
    internal class MultiEntryTranslationDefinesInspector : UEditor
    {
        protected override void OnHeaderGUI()
        {

        }

        public override void OnInspectorGUI()
        {
            //For debug purposes
            //using (new EditorGUI.DisabledGroupScope(true))
            //    base.OnInspectorGUI();
        }
    }
}