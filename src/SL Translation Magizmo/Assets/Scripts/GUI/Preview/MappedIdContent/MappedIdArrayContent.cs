using Project.Translation;

namespace Project.GUI.Preview
{
    public class MappedIdArrayContent : MappedIdContent
    {
        public override string[] GetContent(TranslationManager manager, string id)
        {
            if (!manager.File.Entries.TryGetValue(id, out var txt))
                return new string[0];

            return txt.content.EntryContentToArray();
        }
    }
}