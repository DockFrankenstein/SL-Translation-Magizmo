using System.Linq;

namespace Project.GUI.Preview
{
    public class MappedIdArrayContent : MappedIdContent
    {
        public override string[] GetContent(GetContentArgs args, object context) =>
            args.normalContent
                .SelectMany(x => x.EntryContentToArray())
                .ToArray();
    }
}