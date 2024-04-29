using System;
using System.Linq;
using UnityEngine;

namespace Project.GUI.Preview
{
    public class MappedIdColored : MappedIdContent<MappedIdColored.Context>
    {
        public bool useRichText = true;

        public override string[] GetContentWithContext(GetContentArgs args, Context context) =>
            args.normalContent
            .Select(x => string.IsNullOrWhiteSpace(x) ? args.defaultContent : x)
            .Select(x => $"<color=#{ColorUtility.ToHtmlStringRGBA(context.color)}>{GetContent(x)}</color>")
            .ToArray();

        string GetContent(string x)
        {
            if (useRichText)
                return x;

            return x
                .Replace(">", "<space=0>>");
        }

        [Serializable]
        public class Context
        {
            public Color color;
        }
    }
}