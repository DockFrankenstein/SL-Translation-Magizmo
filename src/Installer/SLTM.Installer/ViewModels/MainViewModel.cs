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

        Installer.OnProcessBegin += () =>
        {
            switch (installer.AppMode)
            {
                case InstallerApp.Mode.Uninstall:
                    installer.OnException += _ => OnUpdateFail();
                    break;
                default:
                    installer.OnException += _ => OnInstallFail();
                    break;
            };
        };

        switch (installer.AppMode)
        {
            case InstallerApp.Mode.Update:
                Installer.OnProcessFinish += OnUpdateFinish;
                break;
            default:
                Installer.OnProcessFinish += OnInstallFinish;
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
                    new FinishPageViewModel(installer)
                    {
                        Header = "Uninstall Successfull",
                        Text = "Successfully uninstalled SL: Translation Magizmo. Please close this application to finalize.",
                        ShowLaunch = false,
                    },
                ];
                break;
            case InstallerApp.Mode.Update:
                Pages =
                [
                    ProgressPage,
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
            new FinishPageViewModel(Installer)
            {
                Header = "Installation Finished",
                Text = "SL: Translation Magizmo was successfully installed :)",
            },
        ];

        UpdateNavigation();
    }

    void OnInstallFail()
    {
        Installer.OnProcessFinish -= OnInstallFinish;

        Pages =
        [
            new FinishPageViewModel(Installer)
            {
                Header = "Installation Unsuccessfull",
                Text = "There was an error :(\nPlease restart the wizzard and try again or report the issue.",
                ShowLaunch = false,
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
            new FinishPageViewModel(Installer)
            {
                Header = "Uninstalling Unsuccessfull",
                Text = "There was an error :(\nPlease restart the wizzard and try again or report the issue.",
                ShowLaunch = false,
            },
        ];

        UpdateNavigation();
    }

    void OnUpdateFinish()
    {
        Installer.OnException -= _ => OnUpdateFail();

        Pages =
        [
            new FinishPageViewModel(Installer)
            {
                Header = "Update Completed",
                Text = "Application has been successfully updated to the latest version!",
            },
        ];

        UpdateNavigation();
    }

    void OnUpdateFail()
    {
        Installer.OnProcessFinish -= OnUpdateFinish;

        Pages =
        [
            new FinishPageViewModel(Installer)
            {
                Header = "Updating Unsuccessfull",
                Text = "There was an error :(\nPlease try again or report the issue.",
                ShowLaunch = false,
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
