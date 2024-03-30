using System;

namespace SLTM.Installer.ViewModels
{
    public abstract class PageViewModelBase : ViewModelBase
    {
        public virtual ButtonData NextButton { get; } = null;
        public virtual ButtonData BackButton { get; } = null;

        public virtual void OnOpenPage() { }
        public virtual void OnClosePage() { }

        public class ButtonData
        {
            public ButtonData() : base() { }
            public ButtonData(string content) : this()
            {
                this.content = content;
            }

            public ButtonData(string content, bool? overriteEnable) : this(content)
            {
                this.overriteEnable = overriteEnable;
            }

            public bool? overriteEnable = null;
            public bool useNavigation = true;
            public string content = null;

            public Action OnClick;

            public static ButtonData ForceDisabled => new ButtonData()
            {
                overriteEnable = false,
            };
        }
    }
}