using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Serialization;
using UnityEditor;
using UnityEngine;
using qASIC;
using Unity.VisualScripting.YamlDotNet.Serialization;
using static Codice.CM.Common.CmCallContext;

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