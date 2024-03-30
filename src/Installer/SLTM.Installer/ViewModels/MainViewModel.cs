using Avalonia.Interactivity;
using MsBox.Avalonia;
using ReactiveUI;
using SLTM.Installer.Services;
using System;
using System.Windows.Input;

namespace SLTM.Installer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(InstallerApp installer)
    {
        Installer = installer;

        ProgressPage = new ProgressViewModel(installer);

        ProgressPage.OnDownloadError += e =>
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard(new MsBox.Avalonia.Dto.MessageBoxStandardParams()
                {
                    ContentHeader = "Download Error",
                    ContentTitle = "Download Error",
                    ContentMessage = e.ToString(),
                    ButtonDefinitions = MsBox.Avalonia.Enums.ButtonEnum.Ok,
                    Width = 500,
                    Height = 300,
                });

            box.ShowAsync();

            Pages =
            [
                new FinishPageViewModel()
                {
                    Header = "Installation Unsuccessfull",
                    Text = "There was an error :(\nPlease restart the wizzard and try again or report the issue.",
                },
            ];

            UpdateNavigation();
        };

        ProgressPage.OnFinish += () =>
        {
            Pages =
            [
                new FinishPageViewModel()
                {
                    Header = "Installation Finished",
                    Text = "SL: Translation Magizmo was successfully installed :)",
                },
            ];

            UpdateNavigation();
        };

        Pages =
        [
            new WelcomePageViewModel(),
            new PickLocationPageViewModel(installer),
            new OptionsSelectViewModel(installer),
            ProgressPage,
        ];

        NextCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage?.NextButton?.OnClick != null)
            {
                CurrentPage.NextButton.OnClick();
                return;
            }

            _page++;
            UpdateNavigation();
        });

        BackCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage?.BackButton?.OnClick != null)
            {
                CurrentPage.BackButton.OnClick();
                return;
            }

            _page--;
            UpdateNavigation();
        });

        _currentPage = Pages[0];
        Pages[0].OnOpenPage();

        UpdateNavigation();
    }

    public InstallerApp Installer { get; private set; }

    int _page = 0;
    private PageViewModelBase[] Pages;

    public ProgressViewModel ProgressPage { get; init; }

    PageViewModelBase _currentPage;
    public PageViewModelBase CurrentPage
    {
        get => _currentPage;
        set
        {
            if (value != _currentPage)
            {
                _currentPage.OnClosePage();
                this.RaiseAndSetIfChanged(ref _currentPage, value);
                value.OnOpenPage();
            }
        }
    }

    public ICommand NextCommand { get; }
    public ICommand BackCommand { get; }

    string _nextButtonContent;
    public string NextButtonContent
    {
        get => _nextButtonContent;
        set => this.RaiseAndSetIfChanged(ref _nextButtonContent, value);
    }

    string _backButtonContent;
    public string BackButtonContent
    {
        get => _backButtonContent;
        set => this.RaiseAndSetIfChanged(ref _backButtonContent, value);
    }

    bool _enableNext = true;
    public bool EnableNext
    {
        get => _enableNext;
        set => this.RaiseAndSetIfChanged(ref _enableNext, value);
    }

    bool _enableBack = true;
    public bool EnableBack
    {
        get => _enableBack;
        set => this.RaiseAndSetIfChanged(ref _enableBack, value);
    }

    void UpdateNavigation()
    {
        _page = Math.Clamp(_page, 0, Pages.Length - 1);

        CurrentPage = Pages[_page];

        var back = CurrentPage.BackButton;
        var next = CurrentPage.NextButton;

        BackButtonContent = back?.content ?? "Back";
        NextButtonContent = next?.content ?? "Next";

        EnableBack = _page > 0;
        EnableNext = _page < Pages.Length - 1;

        EnableBack = back?.overriteEnable ?? EnableBack;
        EnableNext = next?.overriteEnable ?? EnableNext;
    }
}
