using Project.Serialization;
using UnityEditor;
using UnityEngine;
using qASIC;

namespace Project.Editor.Windows
{
    public class CsvTester : EditorWindow
    {
        [MenuItem("Window/Project/Csv Tester")]
        public static void OpenWindow()
        {
            var window = CreateWindow<CsvTester>("Csv Tester");        
            window.Show();
        }

        [SerializeField] CsvParser parser = new CsvParser();
        [SerializeField] string txt;

        [SerializeField] int columnIndex;

        private void OnGUI()
        {
            parser.CellSeparator = EditorGUILayout.TextField("Cell Separator", parser.CellSeparator);
            parser.CellWrapping = EditorGUILayout.TextField("Cell Wrapping", parser.CellWrapping);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                    GUILayout.Space((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 7f);
            }

            var txtRect = GUILayoutUtility.GetLastRect();
            GUI.Label(txtRect.SetHeight(EditorGUIUtility.singleLineHeight), "Content");

            txt = EditorGUI.TextArea(txtRect.BorderTop(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), txt);

            EditorGUILayout.Space();

            columnIndex = Mathf.Max(0, EditorGUILayout.IntField($"{Table2D.GetColumnName((uint)columnIndex)}", columnIndex));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Text Deserialization"))
            {
                Debug.Log(parser.Deserialize(txt));
            }

            if (GUILayout.Button("Test Serialization"))
            {
                var table = parser.Deserialize(txt);
                var newTxt = parser.Serialize(table);

                Debug.Log(table);
                Debug.Log($"Serialized result: {newTxt}");
            }
        }
    }
}