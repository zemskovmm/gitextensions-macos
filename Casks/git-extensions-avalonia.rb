cask "git-extensions-avalonia" do
  version "0.1.1"
  sha256 "a2fa31e793da4a50b2bc877b70857f2214b4a7191c323f97c63cd6f0061444d2"

  url "https://github.com/zemskovmm/gitextensions/releases/download/avalonia-macos-v#{version}/GitExtensions-Avalonia-#{version}-osx-arm64.app.zip"
  name "Git Extensions Avalonia"
  desc "Ad-hoc-signed Apple Silicon development preview of Git Extensions"
  homepage "https://github.com/zemskovmm/gitextensions"

  livecheck do
    skip "Versioned macOS preview channel"
  end

  depends_on arch: :arm64
  depends_on macos: :sonoma

  app "Git Extensions Avalonia.app"

  caveats <<~EOS
    This development preview is ad-hoc signed, not Developer ID signed or
    notarized. macOS may block the first launch. If you accept that risk,
    use Privacy & Security > Open Anyway; do not remove quarantine from
    unrelated applications.
  EOS
end
