#!/usr/bin/env bash
set -euo pipefail

if [[ "$(uname -s)" != "Darwin" ]]; then
    echo "macOS package-signature test skipped outside macOS"
    exit 0
fi

for tool in codesign ditto iconutil mktemp plutil sips; do
    if ! command -v "$tool" >/dev/null 2>&1; then
        echo "error: required command '$tool' is not installed" >&2
        exit 1
    fi
done

repository_root=$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)
work_directory=$(mktemp -d)
cleanup()
{
    rm -rf -- "$work_directory"
}
trap cleanup EXIT

publish_directory="$work_directory/publish"
archive="$work_directory/GitExtensions-Avalonia-test-osx-arm64.app.zip"
extracted_directory="$work_directory/extracted"
app="$extracted_directory/Git Extensions Avalonia.app"
mkdir -p "$publish_directory"
cp /usr/bin/true "$publish_directory/GitExtensions.Avalonia"
printf '<doc />\n' > "$publish_directory/GitExtensions.Avalonia.xml"

bash "$repository_root/eng/avalonia/package-macos-app.sh" \
    "$publish_directory" "$archive" "0.0.0" "0.0.0"
ditto -x -k "$archive" "$extracted_directory"

codesign --verify --deep --strict "$app"
signature=$(codesign -dvv "$app" 2>&1)
if [[ "$signature" != *"Identifier=com.github.gitextensions.GitExtensions.Avalonia"* ]]; then
    echo "error: bundle signature does not use the application identifier" >&2
    exit 1
fi
if [[ "$signature" != *"Signature=adhoc"* ]]; then
    echo "error: bundle is not ad-hoc signed" >&2
    exit 1
fi

test "$(plutil -extract CFBundleIdentifier raw "$app/Contents/Info.plist")" = \
    "com.github.gitextensions.GitExtensions.Avalonia"
cmp "$repository_root/LICENSE.md" "$app/Contents/Resources/LICENSE.md"

icon_file=$(plutil -extract CFBundleIconFile raw "$app/Contents/Info.plist")
icon="$app/Contents/Resources/$icon_file"
test -s "$icon"
iconset="$work_directory/extracted.iconset"
iconutil -c iconset "$icon" -o "$iconset"
for size in 16 32 128 256 512; do
    for scale in 1 2; do
        pixels=$((size * scale))
        if (( pixels > 512 )); then
            continue
        fi
        suffix=""
        if (( scale == 2 )); then
            suffix="@2x"
        fi
        image="$iconset/icon_${size}x${size}${suffix}.png"
        dimensions=$(sips -g pixelWidth -g pixelHeight "$image")
        [[ "$dimensions" == *"pixelWidth: $pixels"* ]]
        [[ "$dimensions" == *"pixelHeight: $pixels"* ]]
        # iconutil's legacy 16/32px extraction changes translucent RGB values.
        # Compare pixels for PNG-backed representations; check legacy sizes above.
        if (( scale == 1 && size <= 32 )); then
            continue
        fi
        # PNG metadata changes on extraction; uncompressed BMP normalizes it.
        sips -s format bmp "$repository_root/setup/assets/Logo/git-extensions-logo-${pixels}px.png" \
            --out "$work_directory/expected.bmp" >/dev/null
        sips -s format bmp "$image" --out "$work_directory/actual.bmp" >/dev/null
        cmp "$work_directory/expected.bmp" "$work_directory/actual.bmp"
    done
done
echo "macOS package-signature and official-icon test passed"
