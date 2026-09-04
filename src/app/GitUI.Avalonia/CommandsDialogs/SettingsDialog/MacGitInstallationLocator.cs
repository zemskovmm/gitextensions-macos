using GitCommands;
using GitCommands.Git;

namespace GitUI.CommandsDialogs.SettingsDialog;

internal sealed record GitInstallation(string Path, GitVersion Version, bool IsCurrent);

internal interface IGitInstallationLocator
{
    IReadOnlyList<GitInstallation> Find(string effectiveCommand, string? environmentPath);
}

internal sealed class MacGitInstallationLocator : IGitInstallationLocator
{
    private static readonly string[] StandardPaths =
    [
        "/opt/homebrew/bin/git",
        "/usr/local/bin/git",
        "/usr/local/git/bin/git",
        "/opt/local/bin/git",
        "/usr/bin/git",
    ];

    private readonly Func<string, bool> _fileExists;
    private readonly Func<string, string?> _getVersion;

    public MacGitInstallationLocator()
        : this(File.Exists, GetVersion)
    {
    }

    internal MacGitInstallationLocator(Func<string, bool> fileExists, Func<string, string?> getVersion)
    {
        _fileExists = fileExists;
        _getVersion = getVersion;
    }

    public IReadOnlyList<GitInstallation> Find(string effectiveCommand, string? environmentPath)
    {
        string? currentPath = ResolveCurrentPath(effectiveCommand, environmentPath);
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
        List<GitInstallation> installations = [];

        foreach (string path in GetCandidatePaths(effectiveCommand, environmentPath))
        {
            if (!seen.Add(path) || !_fileExists(path))
            {
                continue;
            }

            try
            {
                string? output = _getVersion(path);
                if (!string.IsNullOrWhiteSpace(output))
                {
                    installations.Add(new(path, new GitVersion(output), PathsEqual(path, currentPath)));
                }
            }
            catch
            {
                // Git discovery is deliberately best-effort.
            }
        }

        return installations;
    }

    private IEnumerable<string> GetCandidatePaths(string effectiveCommand, string? environmentPath)
    {
        if (Path.IsPathFullyQualified(effectiveCommand))
        {
            yield return Path.GetFullPath(effectiveCommand);
        }
        else
        {
            foreach (string directory in GetPathDirectories(environmentPath))
            {
                yield return Path.Join(directory, effectiveCommand);
            }
        }

        foreach (string path in StandardPaths)
        {
            yield return path;
        }
    }

    private string? ResolveCurrentPath(string effectiveCommand, string? environmentPath)
    {
        if (Path.IsPathFullyQualified(effectiveCommand))
        {
            string path = Path.GetFullPath(effectiveCommand);
            return _fileExists(path) ? path : null;
        }

        return GetPathDirectories(environmentPath)
            .Select(directory => Path.Join(directory, effectiveCommand))
            .FirstOrDefault(_fileExists);
    }

    private static IEnumerable<string> GetPathDirectories(string? environmentPath)
        => (environmentPath ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static bool PathsEqual(string path, string? other)
        => string.Equals(path, other, StringComparison.OrdinalIgnoreCase);

    private static string? GetVersion(string path)
        => new Executable(path).GetOutput("--version");
}
