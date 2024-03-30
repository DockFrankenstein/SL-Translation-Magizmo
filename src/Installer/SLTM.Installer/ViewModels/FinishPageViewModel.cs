using ReactiveUI;
using System;

namespace SLTM.Installer.ViewModels
{
    public class FinishPageViewModel : PageViewModelBase
    {
        public FinishPageViewModel()
        {
            _nextButton = new ButtonData()
            {
                content = "Finish",
                overriteEnable = true,
                useNavigation = false,
            };

            _backButton = new ButtonData()
            {
                overriteEnable = false,
                useNavigation = false,
            };

            _nextButton.OnClick += () =>
            {
                Environment.Exit(0);
            };
        }

        ButtonData _nextButton;
        public override ButtonData NextButton => _nextButton;

        ButtonData _backButton;
        public override ButtonData BackButton => _backButton;

        private string _header;
        public string Header
        {
            get => _header;
            set => this.RaiseAndSetIfChanged(ref _header, value);
        }

        private string _text;
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
    }
}