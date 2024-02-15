using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

namespace Project.AppTools.Tools
{
    public class SetAllAppTool : AppToolBase
    {
        public override string ToolName => "Set All";

        [SerializeField] string[] entriesIdBlacklist = new string[0];

        TextField _contentField;
        Button _applyButton;
        Button _cancelButton;

        protected override void SetupTool()
        {
            var root = document.rootVisualElement;

            _contentField = root.Q<TextField>("content");
            _applyButton = root.Q<Button>("apply");
            _cancelButton = root.Q<Button>("cancel");

            _applyButton.clicked += _applyButton_clicked;
            _cancelButton.clicked += _cancelButton_clicked;
        }

        private void _cancelButton_clicked()
        {
            CloseTool();
        }

        private void _applyButton_clicked()
        {
            var fields = TranslationManager.CurrentVersion.GetMappedFields()
                .Where(x => !entriesIdBlacklist.Contains(x.id));

            var file = TranslationManager.file;
            var content = _contentField.value;

            foreach (var item in fields)
            {
                if (!file.Entries.ContainsKey(item.id))
                    file.Entries.Add(item.id, new Translation.Data.SaveFile.EntryData(item));

                file.Entries[item.id].content = content;
            }

            CloseTool();
        }
    }
}
