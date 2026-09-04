using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using GitExtensions.Shims.WinForms;

namespace GitUI.Compat;

/// <summary>
///  Implements portable shell actions through the XDG desktop portal on Linux and the native
///  Avalonia launcher on other platforms.
/// </summary>
public sealed class AvaloniaOsShell : IOsShell
{
    private readonly IClassicDesktopStyleApplicationLifetime _desktop;
    private readonly IXdgDesktopPortal _portal;
    private readonly Func<bool> _isLinux;
    private readonly Func<bool> _isMacOS;
    private readonly Func<ProcessStartInfo, bool> _startProcess;

    public AvaloniaOsShell(IClassicDesktopStyleApplicationLifetime desktop, IXdgDesktopPortal portal)
        : this(
            desktop,
            portal,
            OperatingSystem.IsLinux,
            OperatingSystem.IsMacOS,
            startInfo => Process.Start(startInfo) is not null)
    {
    }

    internal AvaloniaOsShell(
        IClassicDesktopStyleApplicationLifetime desktop,
        IXdgDesktopPortal portal,
        Func<bool> isLinux,
        Func<bool>? isMacOS = null,
        Func<ProcessStartInfo, bool>? startProcess = null)
    {
        _desktop = desktop;
        _portal = portal;
        _isLinux = isLinux;
        _isMacOS = isMacOS ?? (() => false);
        _startProcess = startProcess ?? (_ => false);
    }

    public bool TryLaunch(string target, OsShellLaunchKind kind)
        => DispatcherPump.Wait(() => TryLaunchAsync(target, kind));

    internal async Task<bool> TryLaunchAsync(string target, OsShellLaunchKind kind)
    {
        if (_isLinux())
        {
            return await _portal.TryLaunchAsync(target, kind);
        }

        if (_isMacOS() && kind == OsShellLaunchKind.OpenUri)
        {
            return TryLaunchMacOSUri(target);
        }

        TopLevel? topLevel = _desktop.MainWindow;
        if (topLevel is null)
        {
            return false;
        }

        return kind switch
        {
            OsShellLaunchKind.OpenUri => Uri.TryCreate(target, UriKind.Absolute, out Uri? uri)
                && await topLevel.Launcher.LaunchUriAsync(uri),
            OsShellLaunchKind.ShowInDirectory => await LaunchPathAsync(
                topLevel,
                Path.GetDirectoryName(Path.GetFullPath(target))),
            _ => await LaunchPathAsync(topLevel, target),
        };
    }

    private bool TryLaunchMacOSUri(string target)
    {
        if (!Uri.TryCreate(target, UriKind.Absolute, out _))
        {
            return false;
        }

        ProcessStartInfo startInfo = new("/usr/bin/open")
        {
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add(target);

        try
        {
            return _startProcess(startInfo);
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> LaunchPathAsync(TopLevel topLevel, string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        string fullPath = Path.GetFullPath(path);
        IStorageItem? item = Directory.Exists(fullPath)
            ? await topLevel.StorageProvider.TryGetFolderFromPathAsync(fullPath)
            : await topLevel.StorageProvider.TryGetFileFromPathAsync(fullPath);
        return item is not null && await topLevel.Launcher.LaunchFileAsync(item);
    }
}
