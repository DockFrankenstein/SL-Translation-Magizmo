using Avalonia.Interactivity;
using MsBox.Avalonia;
using ReactiveUI;
using SLTM.Installer.Services;
using SLTM.Installer.Views;
using System;
using System.Windows.Input;

namespace SLTM.Installer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(InstallerApp installer)
    {
        Installer = installer;

        installer.OnException += e =>
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
        };

        ProgressPage = new ProgressViewModel(installer);

        ProgressPage.OnBegin += () =>
        {
            switch (installer.AppMode)
            {
                default:
                    installer.OnException += _ => OnInstallFail();
                    break;
            };
        };

        switch (installer.AppMode)
        {
            default:
                ProgressPage.OnFinish += OnInstallFinish;
                break;
        }

        switch (installer.AppMode)
        {
            case InstallerApp.Mode.Uninstall:
                UninstallWelcome = new UninstallConfirmViewModel(installer);
                UninstallWelcome.OnStartUninstall += () =>
                {
                    installer.OnException += _ => OnUninstallFail();
                };

                UninstallWelcome.OnFinishUninstall += OnUninstallFinish;

                Pages =
                [
                    UninstallWelcome,
                    new FinishPageViewModel()
                    {
                        Header = "Uninstall Successfull",
                        Text = "Successfully uninstalled SL: Translation Magizmo. Please close this application to finalize.",
                    },
                ];
                break;
            case InstallerApp.Mode.Update:
                Pages =
                [
                    new FinishPageViewModel()
                    {
                        Header = "Update",
                        Text = "Updating doesn't work yet :/",
                    },
                ];
                break;
            default:
                Pages =
                [
                    new WelcomePageViewModel(),
                    new PickLocationPageViewModel(installer),
                    new OptionsSelectViewModel(installer),
                    ProgressPage,
                ];
                break;
        }


        NextCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage?.NextButton?.OnClick != null)
            {
                CurrentPage.NextButton.OnClick();
            }

            if (CurrentPage?.NextButton?.useNavigation != false)
            {
                _page++;
                UpdateNavigation();
            }
        });

        BackCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage?.BackButton?.OnClick != null)
            {
                CurrentPage.BackButton.OnClick();
            }

            if (CurrentPage?.BackButton?.useNavigation != false)
            {
                _page--;
                UpdateNavigation();
            }
        });

        _currentPage = Pages[0];
        Pages[0].OnOpenPage();

        UpdateNavigation();
    }

    public InstallerApp Installer { get; private set; }

    int _page = 0;
    private PageViewModelBase[] Pages;

    public UninstallConfirmViewModel UninstallWelcome { get; private set; }
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

    void OnInstallFinish()
    {
        switch (Installer.AppMode)
        {
            default:
                Installer.OnException -= _ => OnInstallFail();
                break;
        };

        Pages =
        [
            new FinishPageViewModel()
                {
                    Header = "Installation Finished",
                    Text = "SL: Translation Magizmo was successfully installed :)",
                },
            ];

        UpdateNavigation();
    }

    void OnInstallFail()
    {
        ProgressPage.OnFinish -= OnInstallFinish;

        Pages =
        [
            new FinishPageViewModel()
            {
                Header = "Installation Unsuccessfull",
                Text = "There was an error :(\nPlease restart the wizzard and try again or report the issue.",
            },
        ];

        UpdateNavigation();
    }

    void OnUninstallFinish()
    {
        Installer.OnException -= _ => OnUninstallFail();
    }

    void OnUninstallFail()
    {
        UninstallWelcome.OnFinishUninstall -= OnUninstallFinish;
        Pages =
        [
            new FinishPageViewModel()
            {
                Header = "Uninstalling Unsuccessfull",
                Text = "There was an error :(\nPlease restart the wizzard and try again or report the issue.",
            },
        ];

        UpdateNavigation();
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
