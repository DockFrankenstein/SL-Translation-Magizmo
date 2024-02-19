using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class ErrorWindow : MonoBehaviour
    {
        public string defaultHeader = "There was an error";
        [EditorButton(nameof(DebugPrompt))]
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

        void DebugPrompt()
        {
            if (Application.isPlaying)
            {
                var header = $"Debug Error ({PromptsQueue.Count})";
                var content = "This is a debug error.";

                CreatePrompt(header, content);
            }
        }

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