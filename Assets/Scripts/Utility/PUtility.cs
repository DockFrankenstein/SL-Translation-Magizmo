using System.Linq;

namespace Project
{
    public static partial class PUtility
    { 
        public static string GenerateDisplayName(string id)
        {
            return string.Join(" ", id
                .Split('_')
                .Where(x => x.Length > 0)
                .Select(x => $"{x[0].ToString().ToUpper()}{x.Substring(1, x.Length - 1)}"));
        }
    }
}