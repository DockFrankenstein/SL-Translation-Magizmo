using qASIC.Input.Prompts;

namespace qASIC.Input.Map
{
    public interface ISupportsPrompts
    {
        MapItemPromptData GetPromptData();

        string KeysToPromptText(string[] keys);
    }
}