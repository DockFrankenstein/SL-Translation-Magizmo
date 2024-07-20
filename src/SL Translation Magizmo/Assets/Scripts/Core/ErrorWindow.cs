using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Project
{
    public class ErrorWindow : MonoBehaviour
    {
        public string defaultHeader = "There was an error";
        [EditorButton(nameof(CreateDebugPrompt), activityType: ButtonActivityType.OnPlayMode)]
        [TextArea(3, 5)]
        public string defaultContent = "Application ran into a problem";

        public Queue<Prompt> PromptsQueue { get; private set; }  = new Queue<Prompt>();
        public Action<Prompt> OnPromptCreated;

        public void CreatePrompt(string header, string content)
        {
            var prompt = new Prompt(header, content);
            CreatePrompt(prompt);
        }

        public void CreatePrompt(Prompt prompt)
        {
            PromptsQueue.Enqueue(prompt);
            OnPromptCreated?.Invoke(prompt);
        }

        public void CreateImportExceptionPrompt(Exception e) =>
            CreatePrompt("Import Error", $"There was an error while importing.\n{e}");

        public void CreateExportExceptionPrompt(Exception e) =>
            CreatePrompt("Export Error", $"There was an error while exporting.\n{e}");

        void CreateDebugPrompt() =>
            CreatePrompt($"Debug Error ({PromptsQueue.Count})", "This is a debug error.");

        [Serializable]
        public class Prompt
        {
            public Prompt() { }
            public Prompt(string header, string content)
            {
                this.header = header;
                this.content = content;
            }

            public string header = null;
            public string content = null;
        }
    }
}