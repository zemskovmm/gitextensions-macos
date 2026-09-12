#!/usr/bin/env bash
set -euo pipefail

publish_directory=${1:-}
output_archive=${2:-}
bundle_version=${3:-}
display_version=${4:-}

if [[ -z "$publish_directory" || -z "$output_archive" || -z "$bundle_version" || -z "$display_version" ]]; then
    echo "usage: package-macos-app.sh <publish-directory> <output-archive> <bundle-version> <display-version>" >&2
    exit 2
fi

for tool in codesign ditto iconutil mktemp plutil; do
    if ! command -v "$tool" >/dev/null 2>&1; then
        echo "error: required command '$tool' is not installed" >&2
        exit 1
    fi
done

repository_root=$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)
license_file="$repository_root/LICENSE.md"
if [[ ! -f "$license_file" ]]; then
    echo "error: repository GPL license was not found at '$license_file'" >&2
    exit 1
fi

logo_directory="$repository_root/setup/assets/Logo"
for size in 16 32 64 128 256 512; do
    if [[ ! -f "$logo_directory/git-extensions-logo-${size}px.png" ]]; then
        echo "error: official ${size}px logo was not found in '$logo_directory'" >&2
        exit 1
    fi
done

if [[ ! -x "$publish_directory/GitExtensions.Avalonia" ]]; then
    echo "error: publish directory does not contain the GitExtensions.Avalonia executable" >&2
    exit 1
fi

if [[ ! "$bundle_version" =~ ^[0-9]+([.][0-9]+)*$ ]]; then
    echo "error: bundle version must contain only decimal components" >&2
    exit 2
fi

work_directory=$(mktemp -d)
cleanup()
{
    rm -rf -- "$work_directory"
}
trap cleanup EXIT

app_directory="$work_directory/Git Extensions Avalonia.app"
contents_directory="$app_directory/Contents"
macos_directory="$contents_directory/MacOS"
mkdir -p "$macos_directory" "$contents_directory/Resources"
cp -a "$publish_directory/." "$macos_directory/"
chmod +x "$macos_directory/GitExtensions.Avalonia"
cp "$license_file" "$contents_directory/Resources/LICENSE.md"

# Use the upstream renders at their native resolution, including Retina sizes.
iconset="$work_directory/git-extensions-logo.iconset"
mkdir -p "$iconset"
for size in 16 32 128 256 512; do
    cp "$logo_directory/git-extensions-logo-${size}px.png" "$iconset/icon_${size}x${size}.png"
    retina_size=$((size * 2))
    if (( retina_size <= 512 )); then
        cp "$logo_directory/git-extensions-logo-${retina_size}px.png" "$iconset/icon_${size}x${size}@2x.png"
    fi
done
iconutil -c icns "$iconset" -o "$contents_directory/Resources/git-extensions-logo.icns"

cat > "$contents_directory/Info.plist" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "https://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleDisplayName</key>
  <string>Git Extensions Avalonia</string>
  <key>CFBundleExecutable</key>
  <string>GitExtensions.Avalonia</string>
  <key>CFBundleIdentifier</key>
  <string>com.github.gitextensions.GitExtensions.Avalonia</string>
  <key>CFBundleIconFile</key>
  <string>git-extensions-logo.icns</string>
  <key>CFBundleInfoDictionaryVersion</key>
  <string>6.0</string>
  <key>CFBundleName</key>
  <string>Git Extensions Avalonia</string>
  <key>CFBundlePackageType</key>
  <string>APPL</string>
  <key>CFBundleShortVersionString</key>
  <string>$display_version</string>
  <key>CFBundleVersion</key>
  <string>$bundle_version</string>
  <key>LSMinimumSystemVersion</key>
  <string>14.0</string>
  <key>NSHighResolutionCapable</key>
  <true/>
</dict>
</plist>
EOF

plutil -lint "$contents_directory/Info.plist"
codesign --force --deep --sign - "$app_directory"
codesign --verify --deep --strict "$app_directory"

mkdir -p "$(dirname "$output_archive")"
rm -f -- "$output_archive"
ditto -c -k --sequesterRsrc --keepParent "$app_directory" "$output_archive"
