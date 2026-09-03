cask "git-extensions-avalonia" do
  version "0.1.0"
  sha256 "f6f24bcc7449b8413c31218638dbec3100dea1273dc53b8737945940bbf8bdbb"

  url "https://github.com/zemskovmm/gitextensions/releases/download/avalonia-macos-v#{version}/GitExtensions-Avalonia-#{version}-osx-arm64.app.zip"
  name "Git Extensions Avalonia"
  desc "Unsigned Apple Silicon development preview of Git Extensions"
  homepage "https://github.com/zemskovmm/gitextensions"

  depends_on arch: :arm64
  depends_on macos: :sonoma

  app "Git Extensions Avalonia.app"

  caveats <<~EOS
    This is an unsigned, unnotarized development preview. macOS may block
    the first launch. If you accept that risk, use Privacy & Security >
    Open Anyway; do not remove quarantine from unrelated applications.
  EOS
end
