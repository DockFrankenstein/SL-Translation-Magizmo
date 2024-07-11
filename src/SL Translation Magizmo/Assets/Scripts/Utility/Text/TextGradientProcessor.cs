using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Project.Text
{
    [CreateAssetMenu(fileName = "New Text Gradient Processor", menuName = "Scriptable Objects/Text/Text Gradient Processor")]
    public class TextGradientProcessor : TextPostProcessorBase
    {
        public override bool Process(string data, List<TextPostProcessing.SectionData> sections)
        {
            int length = sections.Sum(x => x.text.Length);
            var splitColorHex = data.Split(',');

            if (splitColorHex.Length != 2)
                return false;

            if (!ColorUtility.TryParseHtmlString(splitColorHex[0], out Color startColor) ||
                !ColorUtility.TryParseHtmlString(splitColorHex[1], out Color endColor))
                return false;

            foreach (var item in sections)
            {
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < item.text.Length; i++)
                {
                    var color = Color.Lerp(startColor, endColor, i / Mathf.Max(length, 1f));
                    builder.Append($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{item.text[i]}</color>");
                }

                item.text = builder.ToString();
            }

            return true;
        }
    }
}