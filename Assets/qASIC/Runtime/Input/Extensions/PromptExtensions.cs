using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static qASIC.Input.Prompts.PromptsVariant;
using UnityEngine;

namespace qASIC.Input.Prompts
{
    public static class PromptExtensions
    {
        public static string[] ToDisplayNames(this Prompt[] prompts) =>
            prompts
            .Select(x => x.displayName)
            .ToArray();

        public static Sprite[] ToSprites(this Prompt[] prompts) =>
            prompts
            .Select(x => x.sprite)
            .ToArray();
    }
}
