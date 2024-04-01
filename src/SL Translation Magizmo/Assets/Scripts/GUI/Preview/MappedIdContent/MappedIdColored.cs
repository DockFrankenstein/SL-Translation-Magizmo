using System;
using System.Linq;
using UnityEngine;
using static Project.GUI.Preview.MappedIdColored;

namespace Project.GUI.Preview
{
    public class MappedIdColored : MappedIdContent<Context>
    {
        public override string[] GetContentWithContext(GetContentArgs args, Context context) =>
            args.normalContent
            .Select(x => string.IsNullOrWhiteSpace(x) ? args.defaultContent : x)
            .Select(x => $"<color=#{ColorUtility.ToHtmlStringRGBA(context.color)}>{x}</color>")
            .ToArray();

        [Serializable]
        public class Context
        {
            public Color color;
        }
    }
}