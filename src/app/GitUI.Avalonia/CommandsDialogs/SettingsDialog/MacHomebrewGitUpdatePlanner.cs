using GitCommands.Git;

namespace GitUI.CommandsDialogs.SettingsDialog;

internal sealed record HomebrewGitUpdatePlan(string BrewPath, string GitPath, string Verb)
{
    public string DisplayCommand => $"{BrewPath} {Verb} git";

    public GitInstallation? FindVerifiedInstallation(IEnumerable<GitInstallation> installations)
        => installations.FirstOrDefault(installation =>
            string.Equals(installation.Path, GitPath, StringComparison.OrdinalIgnoreCase)
            && installation.Version >= GitVersion.LastRecommendedVersion);
}

internal sealed class MacHomebrewGitUpdatePlanner
{
    private static readonly string[] _standardBrewPaths =
    [
        "/opt/homebrew/bin/brew",
        "/usr/local/bin/brew",
    ];

    private readonly Func<string, bool> _fileExists;

    public MacHomebrewGitUpdatePlanner()
        : this(File.Exists)
    {
    }

    internal MacHomebrewGitUpdatePlanner(Func<string, bool> fileExists)
    {
        _fileExists = fileExists;
    }

    public HomebrewGitUpdatePlan? Find(string? environmentPath)
    {
        string? brewPath = GetCandidates(environmentPath).FirstOrDefault(_fileExists);
        if (brewPath is null)
        {
            return null;
        }

        string binDirectory = Path.GetDirectoryName(brewPath)!;
        string gitPath = Path.Combine(binDirectory, "git");
        string prefix = Path.GetDirectoryName(binDirectory)!;
        string formulaGitPath = Path.Combine(prefix, "opt", "git", "bin", "git");
        string verb = _fileExists(gitPath) || _fileExists(formulaGitPath) ? "upgrade" : "install";
        return new(brewPath, gitPath, verb);
    }

    private static IEnumerable<string> GetCandidates(string? environmentPath)
    {
        IEnumerable<string> pathCandidates = (environmentPath ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(directory => Path.Combine(directory, "brew"));
        return pathCandidates.Concat(_standardBrewPaths).Distinct(StringComparer.OrdinalIgnoreCase);
    }
}
