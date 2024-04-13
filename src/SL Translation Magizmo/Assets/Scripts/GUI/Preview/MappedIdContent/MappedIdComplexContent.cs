using UnityEngine;
using Project.Translation;
using System;
using System.Collections.Generic;

namespace Project.GUI.Preview
{
    public class MappedIdComplexContent : MappedIdContent
    {
        public Element element;

        public override string[] GetContent(GetContentArgs args, object context)
        {
            return new string[] { GetContent(args.manager, element).Replace("{{", "{").Replace("}}", "}") };
        }

        string GetContent(TranslationManager manager, Element element)
        {
            var txt = element.defaultContent;

            if (!string.IsNullOrEmpty(element.id) &&
                manager.File.Entries.TryGetValue(element.id, out var val) &&
                !string.IsNullOrWhiteSpace(val.content))
                txt = val.content;

            List<string> vals = new List<string>();

            txt = txt.Replace("{", "{{{{")
                .Replace("}", "}}}}");

            foreach (var item in element.dynamicValues)
            {
                if (item == null) continue;
                while (txt.Contains($"{{{vals.Count}}}"))
                    vals.Add($"{{{vals.Count}}}");

                var content = item switch
                {
                    Element.EntryValue entryVal => GetContent(manager, entryVal.entryContent),
                    Element.StaticValue staticVal => staticVal.staticContent.Replace("{", "{{{{").Replace("}", "}}}}"),
                    _ => string.Empty
                };

                vals.Add(content);
                txt = txt.Replace(item.tag.Replace("{", "{{{{").Replace("}", "}}}}"), $"{{{vals.Count - 1}}}");
                Debug.Log($"Replacing {item.tag} to {{{vals.Count - 1}}}");
            }

            txt = string.Format(txt, vals.ToArray());

            return txt;
        }

        [Serializable]
        public class Element
        {
            public string id;
            [TextArea] public string defaultContent;

            [SerializeReference, Subclass(IsList = true)]
            public List<DynamicValue> dynamicValues = new List<DynamicValue>();

            [Serializable]
            public abstract class DynamicValue
            {
                public string tag;
            }

            public class StaticValue : DynamicValue
            {
                public string staticContent;
            }

            public class EntryValue : DynamicValue
            {
                public Element entryContent;
            }
        }
    }
}