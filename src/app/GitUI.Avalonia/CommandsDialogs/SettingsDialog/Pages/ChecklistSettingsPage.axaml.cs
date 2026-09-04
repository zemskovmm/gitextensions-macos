using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Media;
using GitCommands;
using GitCommands.Config;
using GitCommands.DiffMergeTools;
using GitCommands.Git;
using GitExtensions.Extensibility;
using GitExtensions.Extensibility.Git;
using GitExtensions.Extensibility.Settings;
using GitExtensions.Extensibility.Translations;
using GitExtUtils.GitUI.Theming;
using GitUI.CommandsDialogs.SettingsDialog.ShellExtension;
using GitUI.Compat;
using GitUI.HelperDialogs;
using ResourceManager;
using WinFormsShims = GitExtensions.Shims.WinForms;

namespace GitUI.CommandsDialogs.SettingsDialog.Pages;

public sealed partial class ChecklistSettingsPage : SettingsPageWithHeader
{
    private readonly TranslationString _wrongGitVersion =
        new("Git found but version {0} is not supported. Upgrade to version {1} or later");
    private readonly TranslationString _notRecommendedGitVersion =
        new("Git found but version {0} is older than recommended. Upgrade to version {1} or later");
    private readonly TranslationString _gitVersionFound = new("Git {0} is found on your computer.");
    private readonly TranslationString _sshClientNotFound = new("SSH client not found: {0}.");
    private readonly TranslationString _otherSshClient = new("Other SSH client configured: {0}.");
    private readonly TranslationString _linuxToolsSshNotFound =
        new("Linux tools (sh) not found. To solve this problem you can set the correct path in settings.");
    private readonly TranslationString _solveGitCommandFailedCaption = new("Locate git");
    private readonly TranslationString _gitCanBeRun = new("Git can be run using: {0}");
    private readonly TranslationString _gitCanBeRunCaption = new("Locate git");
    private readonly TranslationString _solveGitCommandFailed =
        new("The command to run git could not be determined automatically." + Environment.NewLine +
            "Please make sure that Git for Windows is installed or set the correct command manually.");
    private readonly TranslationString _shellExtRegistered = new("Shell extensions registered properly.");
    private readonly TranslationString _shellExtNoInstalled =
        new("Shell extensions are not installed. Run the installer to install the shell extensions.");
    private readonly TranslationString _shellExtNeedsToBeRegistered =
        new("{0} needs to be registered in order to use the shell extensions.");
    private readonly TranslationString _registryKeyGitExtensionsMissing =
        new("Registry entry missing [Software\\GitExtensions\\InstallDir].");
    private readonly TranslationString _registryKeyGitExtensionsFaulty =
        new("Invalid installation directory stored in [Software\\GitExtensions\\InstallDir].");
    private readonly TranslationString _registryKeyGitExtensionsCorrect =
        new("Git Extensions is properly registered.");
    private readonly TranslationString _plinkputtyGenpageantNotFound =
        new("PuTTY is configured as SSH client but cannot find plink.exe, puttygen.exe or pageant.exe.");
    private readonly TranslationString _puttyConfigured = new("SSH client PuTTY is configured properly.");
    private readonly TranslationString _opensshUsed =
        new("Default SSH client, OpenSSH, will be used. (commandline window will appear on pull, push and clone operations)");
    private readonly TranslationString _languageConfigured = new("The configured language is {0}.");
    private readonly TranslationString _noLanguageConfigured =
        new("There is no language configured for Git Extensions.");
    private readonly TranslationString _noEmailSet =
        new("You need to configure a username and an email address.");
    private readonly TranslationString _emailSet = new("A username and an email address are configured.");
    private readonly TranslationString _mergeToolXConfiguredNeedsCmd =
        new("{0} is configured as mergetool, this is a custom mergetool and needs a custom cmd to be configured.");
    private readonly TranslationString _customMergeToolXConfigured =
        new("There is a custom mergetool configured: {0}");
    private readonly TranslationString _mergeToolXConfigured =
        new("There is a mergetool configured: {0}");
    private readonly TranslationString _linuxToolsSshFound =
        new("Linux tools (sh) found on your computer.");
    private readonly TranslationString _gitNotFound =
        new("Git not found. To solve this problem you can set the correct path in settings.");
    private readonly TranslationString _adviceDiffToolConfiguration =
        new("You should configure a diff tool to show file diff in external program.");
    private readonly TranslationString _diffToolXConfigured =
        new("There is a difftool configured: {0}");
    private readonly TranslationString _configureMergeTool =
        new("You need to configure merge tool in order to solve merge conflicts.");
    private readonly TranslationString _noDiffToolConfiguredCaption = new("Difftool");
    private readonly TranslationString _puttyFoundAuto =
        new("All paths needed for PuTTY could be automatically found and are set.");
    private readonly TranslationString _linuxToolsShNotFound =
        new("The path to linux tools (sh) could not be found automatically." + Environment.NewLine +
            "Please make sure there are linux tools installed (through Git for Windows or cygwin) or set the correct path manually.");
    private readonly TranslationString _linuxToolsShNotFoundCaption = new("Locate linux tools");
    private readonly TranslationString _shCanBeRun = new("Command sh can be run using: {0}sh");
    private readonly TranslationString _shCanBeRunCaption = new("Locate linux tools");
    private readonly TranslationString _gcmDetectedCaption =
        new("Obsolete git-credential-winstore.exe detected");
    private readonly TranslationString _gitRepairHeading = new("Update Git");
    private readonly TranslationString _gitRepairSummary =
        new("Git Extensions is using Git {0} at:{1}{2}{1}{1}Git {3} or later is recommended.");
    private readonly TranslationString _gitRepairUse = new("Use Git {0}");
    private readonly TranslationString _gitRepairDetected = new("Detected Git installations");
    private readonly TranslationString _gitRepairHideDetected = new("Hide Git installations");
    private readonly TranslationString _gitRepairCurrent = new("Current");
    private readonly TranslationString _gitRepairInstall = new("Install or update Git");
    private readonly TranslationString _gitRepairUpdate = new("Update Git with Homebrew");
    private readonly TranslationString _gitRepairUpdateCaption = new("Confirm Git update");
    private readonly TranslationString _gitRepairUpdateConfirmation =
        new("Git Extensions will run:{0}{1}{0}{0}The command runs without a shell and will not use sudo. Continue?");
    private readonly TranslationString _gitRepairUpdateVerificationFailed =
        new("Homebrew finished, but Git {0} or later was not found at {1}. Review the command output, then rescan or choose a Git executable manually.");
    private readonly TranslationString _gitRepairInstructions =
        new("Install or update Git outside Git Extensions. With Homebrew, run one of these commands in Terminal:{0}brew install git{0}brew upgrade git");
    private readonly TranslationString _gitRepairChoose = new("Choose Git executable...");
    private readonly TranslationString _gitRepairRescan = new("Rescan Git installations");

    private const string _putty = "PuTTY";
    private DiffMergeToolConfigurationManager? _diffMergeToolConfigurationManager;

    public ChecklistSettingsPage()
        : this(EmptyServiceProvider.Instance)
    {
    }

    public ChecklistSettingsPage(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        InitializeComponent();
        Text = "Checklist";

        GitFound.Click += GitFound_Click;
        GitFound_Fix.Click += GitFound_Click;
        UserNameSet.Click += UserNameSet_Click;
        UserNameSet_Fix.Click += UserNameSet_Click;
        MergeTool.Click += MergeToolFix_Click;
        MergeTool_Fix.Click += MergeToolFix_Click;
        DiffTool.Click += DiffToolFix_Click;
        DiffTool_Fix.Click += DiffToolFix_Click;
        ShellExtensionsRegistered.Click += ShellExtensionsRegistered_Click;
        ShellExtensionsRegistered_Fix.Click += ShellExtensionsRegistered_Click;
        GitBinFound.Click += GitBinFound_Click;
        GitBinFound_Fix.Click += GitBinFound_Click;
        GitExtensionsInstall.Click += GitExtensionsInstall_Click;
        GitExtensionsInstall_Fix.Click += GitExtensionsInstall_Click;
        SshConfig.Click += SshConfig_Click;
        SshConfig_Fix.Click += SshConfig_Click;
        translationConfig.Click += translationConfig_Click;
        translationConfig_Fix.Click += translationConfig_Click;
        GcmDetectedFix.Click += GcmDetectedFix_Click;
        Rescan.Click += SaveAndRescan_Click;
        CheckAtStartup.Click += CheckAtStartup_CheckedChanged;
        InitializeComplete();
    }

    /// <summary>
        /// TODO: remove this direct dependency to another SettingsPage later when possible.
        /// </summary>
    public SshSettingsPage? SshSettingsPage { get; set; }

    public override bool IsInstantSavePage => true;

    public static SettingsPageReference GetPageReference()
        => new SettingsPageReferenceByType(typeof(ChecklistSettingsPage));

    public override void OnPageShown() => CheckSettings();

    private void translationConfig_Click(object? sender, EventArgs e)
    {
        using FormChooseTranslation frm = new();
        frm.ShowDialog(TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window); // will set Settings.Translation

        PageHost.LoadAll();

        Translator.Translate(this, AppSettings.CurrentTranslation);
        SaveAndRescan_Click(this, EventArgs.Empty);
    }

    internal ChecklistResult Evaluate(CommonLogic commonLogic)
    {
        CheckState gitStatus = CheckState.Invalid;
        string gitMessage = _gitNotFound.Text;
        bool gitAvailable = TryCheck(() =>
        {
            if (!CheckSettingsLogic.CanFindGitCmd())
            {
                return false;
            }

            IGitVersion nativeGitVersion = GitVersion.Current;
            IGitVersion usedGitVersion = commonLogic.Module.IsValidGitWorkingDir()
                ? GitVersion.CurrentVersion(commonLogic.Module.GitExecutable)
                : nativeGitVersion;
            string displayedVersion = nativeGitVersion == usedGitVersion
                ? $"{nativeGitVersion}"
                : $"{nativeGitVersion} / WSL {usedGitVersion}";
            if (usedGitVersion < GitVersion.LastSupportedVersion)
            {
                gitMessage = string.Format(
                    _wrongGitVersion.Text,
                    displayedVersion,
                    GitVersion.LastRecommendedVersion);
                return false;
            }

            if (usedGitVersion < GitVersion.LastRecommendedVersion)
            {
                gitStatus = CheckState.NotRecommended;
                gitMessage = string.Format(
                    _notRecommendedGitVersion.Text,
                    displayedVersion,
                    GitVersion.LastRecommendedVersion);
                return false;
            }

            gitStatus = CheckState.Valid;
            gitMessage = string.Format(_gitVersionFound.Text, displayedVersion);
            return true;
        });
        if (!gitAvailable && gitStatus != CheckState.NotRecommended)
        {
            gitStatus = CheckState.Invalid;
        }

        bool identity = TryCheck(() =>
            !string.IsNullOrEmpty(commonLogic.GitConfigSettingsSet.GlobalSettings.GetValue(SettingKeyString.UserName))
            && !string.IsNullOrEmpty(commonLogic.GitConfigSettingsSet.GlobalSettings.GetValue(SettingKeyString.UserEmail)));
        DiffMergeToolConfigurationManager tools = _diffMergeToolConfigurationManager ??=
            new DiffMergeToolConfigurationManager(
                () => commonLogic.GitConfigSettingsSet.EffectiveSettings);
        string? mergeTool = null;
        bool merge = TryCheck(() =>
        {
            mergeTool = tools.ConfiguredMergeTool;
            return !string.IsNullOrWhiteSpace(mergeTool)
                   && !string.IsNullOrWhiteSpace(tools.GetToolCommand(mergeTool, DiffMergeToolType.Merge));
        });
        string? diffTool = null;
        bool diff = TryCheck(() =>
        {
            diffTool = tools.ConfiguredDiffTool;
            return !string.IsNullOrWhiteSpace(diffTool)
                   && !string.IsNullOrWhiteSpace(tools.GetToolCommand(diffTool, DiffMergeToolType.Diff));
        });
        bool editor = TryCheck(() => !string.IsNullOrEmpty(commonLogic.GetGlobalEditor()));
        bool translation = !string.IsNullOrEmpty(AppSettings.Translation);

        bool windows = OperatingSystem.IsWindows();
        (bool Install, string Message) install = windows
            ? TryEvaluate(
                CheckInstallRegistration,
                (false, _registryKeyGitExtensionsFaulty.Text))
            : (true, string.Empty);
        (bool Shell, string Message) shell = windows
            ? TryEvaluate(
                CheckShellExtensions,
                (false, string.Format(
                    _shellExtNeedsToBeRegistered.Text,
                    ShellExtensionManager.GitExtensionsShellEx32Name)))
            : (true, string.Empty);
        bool gitBin = !windows || TryCheck(() =>
            File.Exists(Path.Join(AppSettings.LinuxToolsDir, "sh.exe"))
            || File.Exists(Path.Join(AppSettings.LinuxToolsDir, "sh"))
            || CheckSettingsLogic.CheckIfFileIsInPath("sh.exe")
            || CheckSettingsLogic.CheckIfFileIsInPath("sh"));
        (bool Ssh, string Message) ssh = windows
            ? TryEvaluate(CheckSsh, (false, _plinkputtyGenpageantNotFound.Text))
            : (true, string.Empty);
        string credentialHelper = string.Empty;
        if (windows)
        {
            TryCheck(() =>
            {
                credentialHelper =
                    commonLogic.GitConfigSettingsSet.GlobalSettings.GetValue(SettingKeyString.CredentialHelper)
                    ?? string.Empty;
                return true;
            });
        }

        bool obsoleteCredentialHelperVisible =
            credentialHelper.Contains("git-credential-winstore.exe", StringComparison.OrdinalIgnoreCase);

        return new ChecklistResult(
            gitStatus,
            identity,
            merge,
            diff,
            editor,
            translation,
            windows,
            install.Install,
            shell.Shell,
            gitBin,
            ssh.Ssh,
            !obsoleteCredentialHelperVisible,
            obsoleteCredentialHelperVisible,
            gitMessage,
            identity ? _emailSet.Text : _noEmailSet.Text,
            merge
                ? string.Format(_mergeToolXConfigured.Text, mergeTool)
                : string.IsNullOrWhiteSpace(mergeTool)
                    ? _configureMergeTool.Text
                    : string.Format(_mergeToolXConfiguredNeedsCmd.Text, mergeTool),
            diff
                ? string.Format(_diffToolXConfigured.Text, diffTool)
                : _adviceDiffToolConfiguration.Text,
            translation
                ? string.Format(_languageConfigured.Text, AppSettings.Translation)
                : _noLanguageConfigured.Text,
            install.Message,
            shell.Message,
            gitBin ? _linuxToolsSshFound.Text : _linuxToolsSshNotFound.Text,
            ssh.Message,
            _gcmDetectedCaption.Text);

        (bool Install, string Message) CheckInstallRegistration()
        {
            string? installDirectory = AppSettings.GetInstallDir();
            if (string.IsNullOrEmpty(installDirectory))
            {
                return (false, _registryKeyGitExtensionsMissing.Text);
            }

            if (installDirectory.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                || !Directory.Exists(installDirectory)
                || (!Debugger.IsAttached
                    && !string.Equals(
                        Path.TrimEndingDirectorySeparator(installDirectory),
                        Path.TrimEndingDirectorySeparator(AppSettings.GetGitExtensionsDirectory()!),
                        StringComparison.OrdinalIgnoreCase)))
            {
                return (false, _registryKeyGitExtensionsFaulty.Text);
            }

            return (true, _registryKeyGitExtensionsCorrect.Text);
        }

        (bool Shell, string Message) CheckShellExtensions()
        {
            if (!ShellExtensionManager.FilesExist())
            {
                return (true, _shellExtNoInstalled.Text);
            }

            return ShellExtensionManager.IsRegistered()
                ? (true, _shellExtRegistered.Text)
                : (false, string.Format(
                    _shellExtNeedsToBeRegistered.Text,
                    ShellExtensionManager.GitExtensionsShellEx32Name));
        }

        (bool Ssh, string Message) CheckSsh()
        {
            if (GitSshHelpers.IsPlink)
            {
                bool valid = File.Exists(AppSettings.Plink)
                    && File.Exists(AppSettings.Puttygen)
                    && File.Exists(AppSettings.Pageant);
                return (valid, valid ? _puttyConfigured.Text : _plinkputtyGenpageantNotFound.Text);
            }

            string sshPath = AppSettings.SshPath;
            if (!string.IsNullOrEmpty(sshPath) && !File.Exists(sshPath))
            {
                return (false, string.Format(_sshClientNotFound.Text, sshPath));
            }

            return (true, string.IsNullOrEmpty(sshPath)
                ? _opensshUsed.Text
                : string.Format(_otherSshClient.Text, sshPath));
        }

        bool TryCheck(Func<bool> check)
        {
            try
            {
                return check();
            }
            catch (Exception exception)
            {
                MessageBoxes.Show(
                    TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window,
                    exception.Message,
                    TranslatedStrings.Error,
                    WinFormsShims.MessageBoxButtons.OK,
                    WinFormsShims.MessageBoxIcon.Error);
                return false;
            }
        }

        T TryEvaluate<T>(Func<T> check, T fallback)
        {
            try
            {
                return check();
            }
            catch (Exception exception)
            {
                MessageBoxes.Show(
                    TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window,
                    exception.Message,
                    TranslatedStrings.Error,
                    WinFormsShims.MessageBoxButtons.OK,
                    WinFormsShims.MessageBoxIcon.Error);
                return fallback;
            }
        }
    }

    protected override void SettingsToPage()
    {
        base.SettingsToPage();
        CheckSettings();
    }

    private static void Render(
        Button status,
        Button repair,
        bool isVisible,
        bool valid,
        string message)
        => Render(
            status,
            repair,
            isVisible,
            valid ? CheckState.Valid : CheckState.Invalid,
            message);

    private static void Render(
        Button status,
        Button repair,
        bool isVisible,
        CheckState state,
        string message)
    {
        status.IsVisible = isVisible;
        if (!isVisible)
        {
            repair.IsVisible = false;
            return;
        }

        switch (state)
        {
            case CheckState.Valid:
                RenderSettingSet(status, repair, message);
                break;
            case CheckState.NotRecommended:
                RenderSettingNotRecommended(status, repair, message);
                break;
            default:
                RenderSettingUnset(status, repair, message);
                break;
        }
    }

    private void SshConfig_Click(object? sender, EventArgs e)
    {
        if (GitSshHelpers.IsPlink)
        {
            if (SshSettingsPage is null)
            {
                return;
            }

            if (SshSettingsPage.AutoFindPuttyPaths())
            {
                MessageBoxes.Show(
                    TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window,
                    _puttyFoundAuto.Text,
                    _putty,
                    WinFormsShims.MessageBoxButtons.OK,
                    WinFormsShims.MessageBoxIcon.Information);
            }
            else
            {
                PageHost.GotoPage(SshSettingsPage.GetPageReference());
            }

            return;
        }

        if (SshSettingsPage is not null)
        {
            PageHost.GotoPage(SshSettingsPage.GetPageReference());
        }
    }

    private void GitExtensionsInstall_Click(object? sender, EventArgs e)
    {
        CheckSettingsLogic.SolveGitExtensionsDir();
        CheckSettings();
    }

    private void GitBinFound_Click(object? sender, EventArgs e)
    {
        if (!CheckSettingsLogic.SolveLinuxToolsDir())
        {
            MessageBoxes.Show(
                TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window,
                _linuxToolsShNotFound.Text,
                _linuxToolsShNotFoundCaption.Text,
                WinFormsShims.MessageBoxButtons.OK,
                WinFormsShims.MessageBoxIcon.Error);
            PageHost.GotoPage(GitSettingsPage.GetPageReference());
            return;
        }

        MessageBoxes.Show(
            TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window,
            string.Format(_shCanBeRun.Text, AppSettings.LinuxToolsDir),
            _shCanBeRunCaption.Text,
            WinFormsShims.MessageBoxButtons.OK,
            WinFormsShims.MessageBoxIcon.Information);
        PageHost.LoadAll(); // apply settings to dialog controls (otherwise the later called SaveAndRescan_Click would overwrite settings again)
        SaveAndRescan_Click(sender, e);
    }

    private static void SetStatusColors(Button settingButton, System.Drawing.Color background)
    {
        settingButton.Background = new SolidColorBrush(AvaloniaThemeResources.ToMediaColor(background));
        settingButton.Foreground = new SolidColorBrush(
            AvaloniaThemeResources.ToMediaColor(ColorHelper.GetTextColor(background)));
    }

    // OtherColors is part of the Windows-only source set; preserve its exact portable color formula here.
    private static System.Drawing.Color GetStatusColor(int red, int green, int blue)
    {
        System.Drawing.Color color = System.Drawing.Color.FromArgb(red, green, blue);
        return WinFormsShims.Application.SystemColorMode == WinFormsShims.SystemColorMode.Dark
            ? color.DimColor()
            : color;
    }

    private void ShellExtensionsRegistered_Click(object? sender, EventArgs e)
    {
        ShellExtensionManager.Register();
        CheckSettings();
    }

    private void DiffToolFix_Click(object? sender, EventArgs e)
    {
        string? diffTool = _diffMergeToolConfigurationManager?.ConfiguredDiffTool;
        if (string.IsNullOrEmpty(diffTool))
        {
            GotoPageGlobalSettings();
            return;
        }

        SaveAndRescan_Click(this, EventArgs.Empty);
    }

    private void MergeToolFix_Click(object? sender, EventArgs e)
    {
        string? mergeTool = _diffMergeToolConfigurationManager?.ConfiguredMergeTool;
        if (string.IsNullOrEmpty(mergeTool))
        {
            GotoPageGlobalSettings();
            return;
        }

        SaveAndRescan_Click(this, EventArgs.Empty);
    }

    private void GotoPageGlobalSettings()
        => PageHost.GotoPage(GitConfigSettingsPage.GetPageReference());

    private void UserNameSet_Click(object? sender, EventArgs e)
        => PageHost.GotoPage(GitConfigSettingsPage.GetPageReference());

    private void GitFound_Click(object? sender, EventArgs e)
    {
        if (OperatingSystem.IsMacOS())
        {
            MacGitInstallationLocator locator = new();
            IReadOnlyList<GitInstallation> installations = locator.Find(
                AppSettings.GitCommand,
                Environment.GetEnvironmentVariable("PATH"));
            GitInstallation? current = installations.FirstOrDefault(installation => installation.IsCurrent);
            if (current is not null && current.Version < GitVersion.LastRecommendedVersion)
            {
                WinFormsShims.IWin32Window? owner = TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window;
                IGitUICommands? uiCommands = ServiceProvider as IGitUICommands;
                MacHomebrewGitUpdatePlanner updatePlanner = new();
                RepairGitOnMac(new MacGitRepairOperations(
                    Discover: () => locator.Find(
                        AppSettings.GitCommand,
                        Environment.GetEnvironmentVariable("PATH")),
                    Present: page => TaskDialog.ShowDialog(owner, page),
                    SelectGit: SelectGit,
                    OpenInstallInstructions: () => OsShellUtil.OpenUrlInDefaultBrowser("https://git-scm.com/download/mac"),
                    RefreshChecklist: () =>
                    {
                        PageHost.LoadAll();
                        SaveAndRescan_Click(sender, e);
                    },
                    ChooseExecutable: () => PageHost.GotoPage(GitSettingsPage.GetPageReference()),
                    PlanHomebrewUpdate: () => uiCommands is not null && owner is not null
                        ? updatePlanner.Find(Environment.GetEnvironmentVariable("PATH"))
                        : null,
                    ConfirmHomebrewUpdate: plan => TaskDialog.ShowDialog(owner, new TaskDialogPage
                    {
                        Caption = _gitRepairUpdateCaption.Text,
                        Text = string.Format(
                            _gitRepairUpdateConfirmation.Text,
                            Environment.NewLine,
                            plan.DisplayCommand),
                        Icon = TaskDialogIcon.Warning,
                        Buttons = { TaskDialogButton.Yes, TaskDialogButton.No },
                        DefaultButton = TaskDialogButton.No,
                        AllowCancel = true,
                        SizeToContent = true,
                    }) == TaskDialogButton.Yes,
                    RunHomebrewUpdate: plan => RunHomebrewUpdate(owner!, uiCommands!, plan),
                    HomebrewVerificationFailed: plan => MessageBoxes.ShowError(
                        owner,
                        string.Format(
                            _gitRepairUpdateVerificationFailed.Text,
                            GitVersion.LastRecommendedVersion,
                            plan.GitPath),
                        _gitRepairHeading.Text)));
                return;
            }
        }

        if (!CheckSettingsLogic.SolveGitCommand())
        {
            MessageBoxes.Show(
                TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window,
                _solveGitCommandFailed.Text,
                _solveGitCommandFailedCaption.Text,
                WinFormsShims.MessageBoxButtons.OK,
                WinFormsShims.MessageBoxIcon.Error);
            PageHost.GotoPage(GitSettingsPage.GetPageReference());
            return;
        }

        string command = PathUtil.TryFindFullPath(AppSettings.GitCommand, out string? fullPath)
            ? fullPath
            : AppSettings.GitCommand;
        MessageBoxes.Show(
            TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window,
            string.Format(_gitCanBeRun.Text, command),
            _gitCanBeRunCaption.Text,
            WinFormsShims.MessageBoxButtons.OK,
            WinFormsShims.MessageBoxIcon.Information);
        PageHost.GotoPage(GitSettingsPage.GetPageReference());
        SaveAndRescan_Click(sender, e);

        bool SelectGit(string path)
        {
            if (CheckSettingsLogic.SolveGitCommand(path))
            {
                return true;
            }

            MessageBoxes.Show(
                TopLevel.GetTopLevel(this) as WinFormsShims.IWin32Window,
                _solveGitCommandFailed.Text,
                _solveGitCommandFailedCaption.Text,
                WinFormsShims.MessageBoxButtons.OK,
                WinFormsShims.MessageBoxIcon.Error);
            return false;
        }

        static bool RunHomebrewUpdate(
            WinFormsShims.IWin32Window owner,
            IGitUICommands uiCommands,
            HomebrewGitUpdatePlan plan)
        {
            ArgumentBuilder arguments = [plan.Verb, "git"];
            return FormProcess.ShowDialog(
                owner,
                uiCommands,
                arguments,
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                input: null,
                useDialogSettings: false,
                process: plan.BrewPath);
        }
    }

    internal MacGitRepairDialog CreateMacGitRepairDialog(
        IReadOnlyList<GitInstallation> installations,
        Action openInstallInstructions,
        HomebrewGitUpdatePlan? updatePlan = null)
    {
        GitInstallation current = installations.First(installation => installation.IsCurrent);
        Dictionary<TaskDialogButton, GitInstallation> installationButtons = [];
        TaskDialogPage page = new()
        {
            Caption = _gitCanBeRunCaption.Text,
            Heading = _gitRepairHeading.Text,
            Text = string.Format(
                _gitRepairSummary.Text,
                current.Version,
                Environment.NewLine,
                current.Path,
                GitVersion.LastRecommendedVersion)
                + Environment.NewLine
                + Environment.NewLine
                + string.Format(_gitRepairInstructions.Text, Environment.NewLine),
            Icon = TaskDialogIcon.Warning,
            AllowCancel = true,
            SizeToContent = true,
            Expander = new TaskDialogExpander
            {
                CollapsedButtonText = _gitRepairDetected.Text,
                ExpandedButtonText = _gitRepairHideDetected.Text,
                Text = string.Join(
                    Environment.NewLine,
                    installations.Select(installation =>
                        $"{(installation.IsCurrent ? $"{_gitRepairCurrent.Text}: " : string.Empty)}{installation.Version} — {installation.Path}")),
            },
        };

        foreach (GitInstallation installation in installations.Where(
                     installation => !installation.IsCurrent && installation.Version >= GitVersion.LastRecommendedVersion))
        {
            TaskDialogCommandLinkButton button = new(
                string.Format(_gitRepairUse.Text, installation.Version),
                installation.Path);
            installationButtons.Add(button, installation);
            page.Buttons.Add(button);
        }

        TaskDialogCommandLinkButton? update = updatePlan is null
            ? null
            : new(_gitRepairUpdate.Text, updatePlan.DisplayCommand);
        if (update is not null)
        {
            page.Buttons.Add(update);
        }

        TaskDialogCommandLinkButton install = new(_gitRepairInstall.Text, allowCloseDialog: false);
        install.Click += (_, _) => openInstallInstructions();
        TaskDialogCommandLinkButton chooseExecutable = new(_gitRepairChoose.Text);
        TaskDialogButton rescan = new(_gitRepairRescan.Text);
        page.Buttons.Add(install);
        page.Buttons.Add(chooseExecutable);
        page.Buttons.Add(rescan);
        page.Buttons.Add(TaskDialogButton.Cancel);
        page.DefaultButton = rescan;
        return new(page, installationButtons, update, install, chooseExecutable, rescan);
    }

    internal void RepairGitOnMac(MacGitRepairOperations operations)
    {
        while (true)
        {
            IReadOnlyList<GitInstallation> installations = operations.Discover();
            GitInstallation? current = installations.FirstOrDefault(installation => installation.IsCurrent);
            if (current is null)
            {
                operations.ChooseExecutable();
                return;
            }

            if (current.Version >= GitVersion.LastRecommendedVersion)
            {
                operations.RefreshChecklist();
                return;
            }

            HomebrewGitUpdatePlan? updatePlan = operations.PlanHomebrewUpdate?.Invoke();
            MacGitRepairDialog dialog = CreateMacGitRepairDialog(
                installations,
                operations.OpenInstallInstructions,
                updatePlan);
            TaskDialogButton result = operations.Present(dialog.Page);
            if (result == dialog.RescanButton)
            {
                continue;
            }

            if (result == dialog.ChooseExecutableButton)
            {
                operations.ChooseExecutable();
            }
            else if (updatePlan is not null && result == dialog.UpdateButton)
            {
                if (operations.ConfirmHomebrewUpdate?.Invoke(updatePlan) != true
                    || operations.RunHomebrewUpdate?.Invoke(updatePlan) != true)
                {
                    return;
                }

                GitInstallation? updated = updatePlan.FindVerifiedInstallation(operations.Discover());
                if (updated is null)
                {
                    operations.HomebrewVerificationFailed?.Invoke(updatePlan);
                }
                else if (operations.SelectGit(updated.Path))
                {
                    operations.RefreshChecklist();
                }
                else
                {
                    operations.ChooseExecutable();
                }
            }
            else if (dialog.InstallationButtons.TryGetValue(result, out GitInstallation? installation))
            {
                if (operations.SelectGit(installation.Path))
                {
                    operations.RefreshChecklist();
                }
                else
                {
                    operations.ChooseExecutable();
                }
            }

            return;
        }
    }

    private void SaveAndRescan_Click(object? sender, EventArgs e)
    {
        using (WaitCursorScope.Enter())
        {
            PageHost.SaveAll();
            PageHost.LoadAll();
            CheckSettings();
        }
    }

    private void CheckAtStartup_CheckedChanged(object? sender, EventArgs e)
        => AppSettings.CheckSettings = CheckAtStartup.IsChecked == true;

    public bool CheckSettings()
    {
        _diffMergeToolConfigurationManager = new DiffMergeToolConfigurationManager(
            () => CheckSettingsLogic.CommonLogic.GitConfigSettingsSet.EffectiveSettings);
        ChecklistResult result = Evaluate(CommonLogic);
        Render(GitFound, GitFound_Fix, isVisible: true, result.GitStatus, result.GitMessage);
        Render(UserNameSet, UserNameSet_Fix, isVisible: true, result.Identity, result.IdentityMessage);
        Render(MergeTool, MergeTool_Fix, isVisible: true, result.MergeTool, result.MergeToolMessage);
        Render(DiffTool, DiffTool_Fix, isVisible: true, result.DiffTool, result.DiffToolMessage);
        Render(
            ShellExtensionsRegistered,
            ShellExtensionsRegistered_Fix,
            result.WindowsChecksVisible,
            result.ShellExtensions,
            result.ShellExtensionsMessage);
        Render(GitBinFound, GitBinFound_Fix, result.WindowsChecksVisible, result.GitBin, result.GitBinMessage);
        Render(
            GitExtensionsInstall,
            GitExtensionsInstall_Fix,
            result.WindowsChecksVisible,
            result.InstallRegistration,
            result.InstallRegistrationMessage);
        Render(SshConfig, SshConfig_Fix, result.WindowsChecksVisible, result.Ssh, result.SshMessage);
        Render(
            translationConfig,
            translationConfig_Fix,
            isVisible: true,
            result.Translation,
            result.TranslationMessage);
        Render(
            GcmDetected,
            GcmDetectedFix,
            result.ObsoleteCredentialHelperVisible,
            result.ObsoleteCredentialHelper,
            result.ObsoleteCredentialHelperMessage);

        if (result.IsValid && AppSettings.CheckSettings)
        {
            AppSettings.CheckSettings = false;
        }

        CheckAtStartup.IsChecked = AppSettings.CheckSettings;
        return result.IsValid;
    }

    /// <summary>
    /// Renders settings as correctly configured.
    /// </summary>
    private static void RenderSettingSet(Button settingButton, Button settingFixButton, string text)
    {
        SetStatusColors(settingButton, GetStatusColor(128, 255, 128));
        settingButton.Content = text;
        settingFixButton.IsVisible = false;
    }

    /// <summary>
    /// Renders settings as misconfigured.
    /// </summary>
    private static void RenderSettingUnset(Button settingButton, Button settingFixButton, string text)
    {
        SetStatusColors(settingButton, GetStatusColor(255, 128, 128));
        settingButton.Content = text;
        settingFixButton.IsVisible = true;
    }

    private static void RenderSettingNotRecommended(Button settingButton, Button settingFixButton, string text)
    {
        SetStatusColors(settingButton, GetStatusColor(255, 255, 128));
        settingButton.Content = text;
        settingFixButton.IsVisible = true;
    }

    private void GcmDetectedFix_Click(object? sender, EventArgs e)
        => OsShellUtil.OpenUrlInDefaultBrowser(
            "https://github.com/gitextensions/gitextensions/wiki/Fix-GitCredentialWinStore-missing");

    internal enum CheckState
    {
        Invalid,
        NotRecommended,
        Valid,
    }

    internal sealed record ChecklistResult(
        CheckState GitStatus,
        bool Identity,
        bool MergeTool,
        bool DiffTool,
        bool Editor,
        bool Translation,
        bool WindowsChecksVisible,
        bool InstallRegistration,
        bool ShellExtensions,
        bool GitBin,
        bool Ssh,
        bool ObsoleteCredentialHelper,
        bool ObsoleteCredentialHelperVisible,
        string GitMessage,
        string IdentityMessage,
        string MergeToolMessage,
        string DiffToolMessage,
        string TranslationMessage,
        string InstallRegistrationMessage,
        string ShellExtensionsMessage,
        string GitBinMessage,
        string SshMessage,
        string ObsoleteCredentialHelperMessage)
    {
        internal bool IsValid
            => GitStatus == CheckState.Valid
               && Identity
               && MergeTool
               && DiffTool
               && Editor
               && Translation
               && InstallRegistration
               && ShellExtensions
               && GitBin
               && Ssh
               && ObsoleteCredentialHelper;
    }
}

internal sealed record MacGitRepairDialog(
    TaskDialogPage Page,
    IReadOnlyDictionary<TaskDialogButton, GitInstallation> InstallationButtons,
    TaskDialogButton? UpdateButton,
    TaskDialogButton InstallButton,
    TaskDialogButton ChooseExecutableButton,
    TaskDialogButton RescanButton);

internal sealed record MacGitRepairOperations(
    Func<IReadOnlyList<GitInstallation>> Discover,
    Func<TaskDialogPage, TaskDialogButton> Present,
    Func<string, bool> SelectGit,
    Action OpenInstallInstructions,
    Action RefreshChecklist,
    Action ChooseExecutable,
    Func<HomebrewGitUpdatePlan?>? PlanHomebrewUpdate = null,
    Func<HomebrewGitUpdatePlan, bool>? ConfirmHomebrewUpdate = null,
    Func<HomebrewGitUpdatePlan, bool>? RunHomebrewUpdate = null,
    Action<HomebrewGitUpdatePlan>? HomebrewVerificationFailed = null);
