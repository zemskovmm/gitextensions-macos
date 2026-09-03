#!/usr/bin/env bash
set -euo pipefail

if [[ "$(uname -s)" != "Darwin" ]]; then
    echo "macOS package-signature test skipped outside macOS"
    exit 0
fi

for tool in codesign ditto mktemp plutil; do
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
echo "macOS package-signature test passed"
