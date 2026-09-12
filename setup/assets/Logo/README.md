This folder contains optimized renders of the Git Extensions logo.

The original SVG file from which the icons were rendered is in the `Artwork` folder.

The macOS app packager (`eng/avalonia/package-macos-app.sh`) derives its native
`.icns` from these PNG renders without resizing them. It includes normal and Retina
representations up to the supplied 512-pixel resolution; no separate logo design or
generated icon binary is maintained.
