using Avalonia.Headless.NUnit;
using GitCommands.Git;
using GitUI.CommandsDialogs.SettingsDialog;
using GitUI.CommandsDialogs.SettingsDialog.Pages;
using GitUI.Compat;
using Microsoft.VisualStudio.Threading;

namespace GitExtensionsTests;

[TestFixture]
public sealed class GitVersionRepairTests
{
    [SetUp]
    public void SetUp()
    {
        GitUI.ThreadHelper.JoinableTaskContext = new JoinableTaskContext();
    }

    [Test]
    public void Locator_finds_the_active_and_homebrew_git_once()
    {
        Dictionary<string, string> versions = new(StringComparer.OrdinalIgnoreCase)
        {
            ["/usr/bin/git"] = "git version 2.50.1 (Apple Git-155)",
            ["/opt/homebrew/bin/git"] = "git version 2.55.0",
        };
        MacGitInstallationLocator locator = new(
            versions.ContainsKey,
            path => versions.GetValueOrDefault(path));

        IReadOnlyList<GitInstallation> installations = locator.Find(
            "git",
            string.Join(Path.PathSeparator, "/usr/bin", "/opt/homebrew/bin", "/usr/bin"));

        installations.Should().SatisfyRespectively(
            installation =>
            {
                installation.Path.Should().Be("/usr/bin/git");
                installation.Version.ToString().Should().Be("2.50.1 (Apple Git-155)");
                installation.IsCurrent.Should().BeTrue();
            },
            installation =>
            {
                installation.Path.Should().Be("/opt/homebrew/bin/git");
                installation.Version.ToString().Should().Be("2.55.0");
                installation.IsCurrent.Should().BeFalse();
            });
    }

    [Test]
    public void Locator_keeps_an_explicit_command_as_the_current_installation()
    {
        Dictionary<string, string> versions = new(StringComparer.OrdinalIgnoreCase)
        {
            ["/custom/git"] = "git version 2.54.0",
            ["/opt/homebrew/bin/git"] = "git version 2.55.0",
        };
        MacGitInstallationLocator locator = new(
            versions.ContainsKey,
            path => versions.GetValueOrDefault(path));

        IReadOnlyList<GitInstallation> installations = locator.Find(
            "/custom/git",
            "/opt/homebrew/bin");

        installations.Should().ContainSingle(installation => installation.IsCurrent)
            .Which.Path.Should().Be("/custom/git");
    }

    [Test]
    public void Homebrew_planner_upgrades_an_installed_formula_from_the_native_prefix()
    {
        HashSet<string> files = new(StringComparer.OrdinalIgnoreCase)
        {
            "/opt/homebrew/bin/brew",
            "/opt/homebrew/bin/git",
        };
        MacHomebrewGitUpdatePlanner planner = new(files.Contains);

        HomebrewGitUpdatePlan? plan = planner.Find(string.Join(Path.PathSeparator, "/usr/bin", "/bin"));

        plan.Should().NotBeNull();
        plan!.BrewPath.Should().Be("/opt/homebrew/bin/brew");
        plan.GitPath.Should().Be("/opt/homebrew/bin/git");
        plan.Verb.Should().Be("upgrade");
        plan.DisplayCommand.Should().Be("/opt/homebrew/bin/brew upgrade git");
    }

    [Test]
    public void Homebrew_planner_installs_git_when_the_formula_is_absent()
    {
        MacHomebrewGitUpdatePlanner planner = new(
            path => path == "/usr/local/bin/brew");

        HomebrewGitUpdatePlan? plan = planner.Find(string.Join(Path.PathSeparator, "/usr/bin", "/bin"));

        plan.Should().NotBeNull();
        plan!.BrewPath.Should().Be("/usr/local/bin/brew");
        plan.GitPath.Should().Be("/usr/local/bin/git");
        plan.Verb.Should().Be("install");
    }

    [Test]
    public void Homebrew_planner_is_unavailable_without_homebrew()
    {
        MacHomebrewGitUpdatePlanner planner = new(_ => false);

        planner.Find(string.Empty).Should().BeNull();
    }

    [Test]
    public void Homebrew_plan_verifies_only_its_recommended_git()
    {
        HomebrewGitUpdatePlan plan = new(
            "/opt/homebrew/bin/brew",
            "/opt/homebrew/bin/git",
            "upgrade");
        GitInstallation apple = new(
            "/usr/bin/git",
            new GitVersion("git version 2.55.0"),
            IsCurrent: true);
        GitInstallation oldHomebrew = new(
            "/opt/homebrew/bin/git",
            new GitVersion("git version 2.52.0"),
            IsCurrent: false);
        GitInstallation updatedHomebrew = oldHomebrew with { Version = new GitVersion("git version 2.55.0") };

        plan.FindVerifiedInstallation([apple, oldHomebrew]).Should().BeNull();
        plan.FindVerifiedInstallation([apple, updatedHomebrew]).Should().BeSameAs(updatedHomebrew);
    }

    [AvaloniaTest]
    public void Repair_dialog_shows_the_current_path_and_a_suitable_alternative()
    {
        GitInstallation current = new(
            "/usr/bin/git",
            new GitVersion("git version 2.50.1 (Apple Git-155)"),
            IsCurrent: true);
        GitInstallation homebrew = new(
            "/opt/homebrew/bin/git",
            new GitVersion("git version 2.55.0"),
            IsCurrent: false);
        ChecklistSettingsPage checklist = new();

        MacGitRepairDialog dialog = checklist.CreateMacGitRepairDialog([current, homebrew], () => { });

        dialog.Page.Text.Should().Contain("/usr/bin/git").And.Contain("2.50.1 (Apple Git-155)").And.Contain("2.53.0");
        dialog.Page.Expander!.Text.Should().Contain("/usr/bin/git").And.Contain("/opt/homebrew/bin/git");
        KeyValuePair<TaskDialogButton, GitInstallation> choice = dialog.InstallationButtons.Should().ContainSingle().Subject;
        choice.Key.Text.Should().Be("Use Git 2.55.0");
        choice.Key.DescriptionText.Should().Be("/opt/homebrew/bin/git");
        choice.Value.Should().BeSameAs(homebrew);
    }

    [AvaloniaTest]
    public void Repair_dialog_offers_safe_installation_guidance_and_rescan()
    {
        bool instructionsOpened = false;
        ChecklistSettingsPage checklist = new();
        GitInstallation current = new(
            "/usr/bin/git",
            new GitVersion("git version 2.50.1 (Apple Git-155)"),
            IsCurrent: true);

        MacGitRepairDialog dialog = checklist.CreateMacGitRepairDialog(
            [current],
            () => instructionsOpened = true);

        dialog.Page.Text.Should().Contain("brew install git").And.Contain("brew upgrade git");
        dialog.InstallationButtons.Should().BeEmpty();
        dialog.InstallButton.AllowCloseDialog.Should().BeFalse();
        dialog.InstallButton.PerformClick();
        instructionsOpened.Should().BeTrue();
        dialog.RescanButton.Text.Should().Be("Rescan Git installations");
        dialog.ChooseExecutableButton.Text.Should().Be("Choose Git executable...");
    }

    [AvaloniaTest]
    public void Repair_dialog_offers_the_exact_homebrew_update_command()
    {
        ChecklistSettingsPage checklist = new();
        GitInstallation current = new(
            "/usr/bin/git",
            new GitVersion("git version 2.50.1 (Apple Git-155)"),
            IsCurrent: true);
        HomebrewGitUpdatePlan update = new(
            "/opt/homebrew/bin/brew",
            "/opt/homebrew/bin/git",
            "upgrade");

        MacGitRepairDialog dialog = checklist.CreateMacGitRepairDialog(
            [current],
            () => { },
            update);

        dialog.UpdateButton.Should().NotBeNull();
        dialog.UpdateButton!.Text.Should().Be("Update Git with Homebrew");
        dialog.UpdateButton.DescriptionText.Should().Be("/opt/homebrew/bin/brew upgrade git");
    }

    [AvaloniaTest]
    public void Repair_flow_rescans_selects_the_new_git_and_refreshes_the_checklist()
    {
        int discoveryCount = 0;
        string? selectedPath = null;
        bool checklistRefreshed = false;
        GitInstallation current = new(
            "/usr/bin/git",
            new GitVersion("git version 2.50.1 (Apple Git-155)"),
            IsCurrent: true);
        GitInstallation homebrew = new(
            "/opt/homebrew/bin/git",
            new GitVersion("git version 2.55.0"),
            IsCurrent: false);
        ChecklistSettingsPage checklist = new();

        checklist.RepairGitOnMac(new MacGitRepairOperations(
            Discover: () => ++discoveryCount == 1 ? [current] : [current, homebrew],
            Present: page => page.Buttons.FirstOrDefault(button => button.Text == "Use Git 2.55.0")
                ?? page.Buttons.Single(button => button.Text == "Rescan Git installations"),
            SelectGit: path =>
            {
                selectedPath = path;
                return true;
            },
            OpenInstallInstructions: () => { },
            RefreshChecklist: () => checklistRefreshed = true,
            ChooseExecutable: () => { }));

        discoveryCount.Should().Be(2);
        selectedPath.Should().Be("/opt/homebrew/bin/git");
        checklistRefreshed.Should().BeTrue();
    }

    [AvaloniaTest]
    public void Repair_flow_runs_verifies_selects_and_refreshes_a_homebrew_update()
    {
        List<string> calls = [];
        GitInstallation current = new(
            "/usr/bin/git",
            new GitVersion("git version 2.50.1 (Apple Git-155)"),
            IsCurrent: true);
        GitInstallation homebrew = new(
            "/opt/homebrew/bin/git",
            new GitVersion("git version 2.55.0"),
            IsCurrent: false);
        HomebrewGitUpdatePlan update = new(
            "/opt/homebrew/bin/brew",
            "/opt/homebrew/bin/git",
            "upgrade");
        int discoveries = 0;
        ChecklistSettingsPage checklist = new();

        checklist.RepairGitOnMac(new MacGitRepairOperations(
            Discover: () => ++discoveries == 1 ? [current] : [current, homebrew],
            Present: page => page.Buttons.Single(button => button.Text == "Update Git with Homebrew"),
            SelectGit: path =>
            {
                calls.Add($"select:{path}");
                return true;
            },
            OpenInstallInstructions: () => { },
            RefreshChecklist: () => calls.Add("refresh"),
            ChooseExecutable: () => { },
            PlanHomebrewUpdate: () => update,
            ConfirmHomebrewUpdate: plan =>
            {
                calls.Add($"confirm:{plan.DisplayCommand}");
                return true;
            },
            RunHomebrewUpdate: plan =>
            {
                calls.Add($"run:{plan.DisplayCommand}");
                return true;
            },
            HomebrewVerificationFailed: _ => calls.Add("verification-failed")));

        calls.Should().Equal(
            "confirm:/opt/homebrew/bin/brew upgrade git",
            "run:/opt/homebrew/bin/brew upgrade git",
            "select:/opt/homebrew/bin/git",
            "refresh");
    }

    [AvaloniaTest]
    public void Repair_flow_does_not_run_homebrew_when_confirmation_is_declined()
    {
        bool ran = false;
        bool selected = false;
        GitInstallation current = new(
            "/usr/bin/git",
            new GitVersion("git version 2.50.1 (Apple Git-155)"),
            IsCurrent: true);
        HomebrewGitUpdatePlan update = new(
            "/opt/homebrew/bin/brew",
            "/opt/homebrew/bin/git",
            "upgrade");
        ChecklistSettingsPage checklist = new();

        checklist.RepairGitOnMac(new MacGitRepairOperations(
            Discover: () => [current],
            Present: page => page.Buttons.Single(button => button.Text == "Update Git with Homebrew"),
            SelectGit: _ => selected = true,
            OpenInstallInstructions: () => { },
            RefreshChecklist: () => { },
            ChooseExecutable: () => { },
            PlanHomebrewUpdate: () => update,
            ConfirmHomebrewUpdate: _ => false,
            RunHomebrewUpdate: _ => ran = true));

        ran.Should().BeFalse();
        selected.Should().BeFalse();
    }

    [AvaloniaTest]
    public void Repair_flow_does_not_change_git_when_cancelled()
    {
        bool selected = false;
        bool refreshed = false;
        GitInstallation current = new(
            "/usr/bin/git",
            new GitVersion("git version 2.50.1 (Apple Git-155)"),
            IsCurrent: true);
        ChecklistSettingsPage checklist = new();

        checklist.RepairGitOnMac(new MacGitRepairOperations(
            Discover: () => [current],
            Present: _ => TaskDialogButton.Cancel,
            SelectGit: _ => selected = true,
            OpenInstallInstructions: () => { },
            RefreshChecklist: () => refreshed = true,
            ChooseExecutable: () => { }));

        selected.Should().BeFalse();
        refreshed.Should().BeFalse();
    }
}
