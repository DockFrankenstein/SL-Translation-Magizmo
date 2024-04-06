using UnityEngine.UIElements;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

namespace Project.AppTools.Tools
{
    public class ReplaceTool : AppToolBase
    {
        public override string ToolName => "Replace";

        [SerializeField] string[] entriesIdBlacklist = new string[0];
        [SerializeField] string regexTutorialLink;

        Toggle _useRegex;
        Button _regexTutorial;
        TextField _from;
        Label _regexError;
        TextField _to;

        Button _applyButton;
        Button _cancelButton;

        protected override void SetupTool()
        {
            base.SetupTool();

            var root = document.rootVisualElement;

            _useRegex = root.Q<Toggle>("regex");
            _regexTutorial = root.Q<Button>("regex-tut");
            _from = root.Q<TextField>("from");
            _regexError = root.Q<Label>("regex-error");
            _to = root.Q<TextField>("to");

            _applyButton = root.Q<Button>("apply");
            _cancelButton = root.Q<Button>("cancel");

            _regexTutorial.clicked += OpenRegexTutorial;

            _useRegex.RegisterValueChangedCallback(args =>
            {
                if (args.target == _useRegex)
                    UpdateRegexError();
            });

            _from.RegisterValueChangedCallback(args =>
            {
                if (args.target == _from)
                    UpdateRegexError();
            });

            _applyButton.clicked += RunTool;
            _cancelButton.clicked += CloseTool;

            UpdateRegexError();
        }

        void OpenRegexTutorial()
        {
            Application.OpenURL(regexTutorialLink);
        }

        void UpdateRegexError()
        {
            bool enabled = false;

            if (_useRegex.value)
            {
                try
                {
                    Regex.Match("", _from.value);
                }
                catch (ArgumentException)
                {
                    enabled = true;
                }
            }

            _regexError.ChangeDispaly(enabled);
        }

        public override void RunTool()
        {
            var fields = TranslationManager.CurrentVersion.GetMappedFields()
                .Where(x => !entriesIdBlacklist.Contains(x.id));

            var file = TranslationManager.File;

            var useRegex = _useRegex.value;
            var from = _from.value;
            var to = _to.value;

            foreach (var item in fields)
            {
                if (!file.Entries.ContainsKey(item.id))
                    file.Entries.Add(item.id, new Translation.Data.SaveFile.EntryData(item));

                var entry = file.Entries[item.id];

                try
                {
                    entry.content = useRegex ?
                        Regex.Replace(entry.content, from, to) :
                        entry.content.Replace(from, to);
                }
                catch { }
            }

            base.RunTool();
        }
    }
}