cask "git-extensions-avalonia" do
  version "0.1.2"
  sha256 "451ab0249fea98de2da370b8ceade08466159fe070924129dc40edcdd64fc8a1"

  url "https://github.com/zemskovmm/gitextensions-macos/releases/download/avalonia-macos-v#{version}/GitExtensions-Avalonia-#{version}-osx-arm64.app.zip"
  name "Git Extensions Avalonia"
  desc "Ad-hoc-signed Apple Silicon development preview of Git Extensions"
  homepage "https://github.com/zemskovmm/gitextensions-macos"

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
