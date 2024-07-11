using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine;

namespace Project.Text
{
    /// <summary>Scriptable Object for applying post processing to text (e.g. effects).</summary>
    [CreateAssetMenu(fileName = "New Text Post Processing", menuName = "Scriptable Objects/Text/Text Postprocessing", order = -100)]
    public class TextPostProcessing : ScriptableObject
    {
        [SerializeField] List<TextPostProcessorBase> postProcessors = new List<TextPostProcessorBase>();

        /// <summary>Applies post processing on text.</summary>
        /// <param name="text">Text to process.</param>
        /// <returns>Processed text.</returns>
        public string ProcessText(string text)
        {
            List<SectionData> data = new List<SectionData>();

            StringBuilder normalText = new StringBuilder();

            bool creatingTag = false;
            StringBuilder tagText = new StringBuilder();
            
            foreach (var c in text)
            {
                switch (c)
                {
                    case '<':
                        if (creatingTag)
                        {
                            normalText.Append("<");
                            normalText.Append(tagText);
                            tagText.Clear();
                        }

                        creatingTag = true;
                        continue;
                    case '>':
                        if (!creatingTag)
                        {
                            normalText.Append(c);
                            continue;
                        }

                        creatingTag = false;

                        if (normalText.Length > 0)
                            data.Add(new SectionData()
                            {
                                text = normalText.ToString(),
                            });

                        var fullTag = tagText.ToString();
                        data.Add(new SectionData()
                        {
                            tagData = string.Join('=', fullTag.Split("=").Skip(1)),
                            isTag = true,
                            isEndTag = fullTag.StartsWith('/'),
                            tagName = fullTag.TrimStart('/').Split("=").First(),
                            fullTag = fullTag,
                        });

                        normalText.Clear();
                        tagText.Clear();
                        continue;
                }

                if (creatingTag)
                {
                    tagText.Append(c);
                    continue;
                }

                normalText.Append(c);
            }

            if (creatingTag)
            {
                normalText.Append('<');
                normalText.Append(tagText);
            }

            if (normalText.Length > 0)
            {
                data.Add(new SectionData()
                {
                    text = normalText.ToString(),
                });
            }

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].isTag && !data[i].isEndTag)
                {
                    ProcessSection(data.Skip(i));
                }
            }

            var finalText = new StringBuilder();
            foreach (var item in data)
            {
                if (item.processed)
                    continue;

                if (item.isTag)
                {
                    finalText.Append($"<{item.fullTag}>");
                    continue;
                }

                finalText.Append(item.text);
            }

            return finalText.ToString();
        }

        void ProcessSection(IEnumerable<SectionData> data)
        {
            List<SectionData> sections = new List<SectionData>();

            SectionData start = data.First();
            SectionData end = null;

            foreach (var item in data)
            {
                if (item.isTag)
                {
                    if (item.isEndTag && start.tagName == item.tagName)
                    {
                        end = item;
                        break;
                    }

                    continue;
                }

                sections.Add(item);
            }

            if (TrySendingToProcessors(start.tagName, start.tagData, sections))
            {
                start.processed = true;

                if (end != null)
                    end.processed = true;
            }
        }

        bool TrySendingToProcessors(string name, string data, List<SectionData> sections)
        {
            var target = postProcessors
                .Where(x => x.TagNames.Contains(name))
                .FirstOrDefault();

            if (target == null)
                return false;

            return target.Process(data, sections);
        }

        public class SectionData
        {
            public string text;
            public string tagName;
            public bool isTag;
            public bool isEndTag;
            public string tagData;
            public string fullTag;

            public bool processed;
        }
    }
}
