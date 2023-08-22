using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Linq;
using qASIC.Input.KeyProviders;
using System.Text.RegularExpressions;
using System;

namespace qASIC.Input.Prompts
{
    [System.Serializable]
    public class PromptsVariant : ISerializationCallbackReceiver
    {
        public PromptsVariant()
        {
            name = "New Prompt Variant";
        }

        public PromptsVariant(string name)
        {
            this.name = name;
        }

        public string[] keyTypes = new string[] { "key_keyboard" };
        public string[] deviceTypes = new string[] { "" };

        public string name;
        [SerializeField] List<Prompt> prompts = new List<Prompt>();

        public Dictionary<string, Prompt> Prompts { get; private set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            prompts = Prompts
                .Select(x => x.Value)
                .ToList();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Prompts = prompts
                .Where(x => x.key != null)
                .GroupBy(x => x.key)
                .Select(x => x.FirstOrDefault())
                .ToDictionary(x => x.key);
        }

        /// <summary>Make sure that there are prompts for every key of this variant's type</summary>
        public void EnsureKeyTypes()
        {
            var keys = keyTypes
                .Select(x => KeyTypeManager.GetProviderByRootPath(x))
                .Where(x => x != null)
                .SelectMany(x => x.KeyPaths.Select(y => $"{x.RootPath}/{y}"));

            Prompts = Prompts
                .Where(x => keys.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var key in keys)
            {
                if (Prompts.ContainsKey(key)) continue;
                Prompts.Add(key, new Prompt(key));
            }
        }

        public Prompt GetPromptFromPath(string keyPath) =>
            Prompts.ContainsKey(keyPath) ?
            Prompts[keyPath] :
            null;

        public Prompt[] GetPromptsFromPaths(List<string> keyPath) =>
            keyPath.Select(GetPromptFromPath)
            .ToArray();

        public Prompt[] GetPromptsFromPaths(params string[] keyPath) =>
            keyPath.Select(GetPromptFromPath)
            .ToArray();

        [Serializable]
        public class Prompt
        {
            public Prompt(string key, string displayName)
            {
                this.key = key;

                displayName = displayName.Replace('\\', '/').Split('/').Last();

                //Add spaces in front of capital letters
                //e.g. UpArrow -> Up Arrow
                displayName = Regex.Replace(displayName, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

                //Remove multiple spaces
                //e.g. Up  Arrow -> Up Arrow
                displayName = Regex.Replace(displayName, @"\s+", " ");

                this.displayName = displayName;
            }

            public Prompt(string key) : this(key, key) { }

            public string key;
            public string displayName;
            public Sprite sprite;

            public override string ToString() =>
                $"{key}, name: {displayName}, sprite: {sprite}";
        }
    }
}
