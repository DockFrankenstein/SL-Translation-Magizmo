using System;
using System.Linq;

namespace qASIC.Options.Menu.BuiltIn
{
    public abstract class OptionsMenuDropdown : OptionsMenuItem
    {
        public object[] values;
    }

    public class OptionsMenuDropdown<T> : OptionsMenuDropdown
    {
        public OptionsMenuDropdown(string name, string displayName, params T[] values)
        {
            this.name = name;
            this.displayName = displayName;
            base.values = values.Select(x => x as object).ToArray();
        }

        public override Type ValueType => typeof(T);
    }
}