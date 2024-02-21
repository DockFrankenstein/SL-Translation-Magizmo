namespace Project.Editor.Translation.Defines.Tools
{
    public abstract class MultiEntryWindowTool
    {
        public abstract string Name { get; }

        public MultiEntryWindow Window { get; set; }

        public virtual void Initialize() { }
        public abstract void OnGUI();
    }
}