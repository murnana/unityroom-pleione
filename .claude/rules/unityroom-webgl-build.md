# unityroom WebGL Build Settings

Required WebGL build settings for submitting to unityroom (<https://unityroom.com/>).

## Required Settings

### Player Settings > Publishing Settings
- **Compression Format: Gzip**
  - Must be Gzip — not Brotli or Disabled
  - Other formats will not work on unityroom (the server handles Gzip decompression)
  - Decompression Fallback can remain OFF (handled server-side by unityroom)

### Player Settings > Resolution
- **Default Canvas Width: 960**
- **Default Canvas Height: 540**
  - Recommended resolution to match unityroom's display area

### Build Settings
- **Development Build: OFF** (release build)
  - Development builds are not minified and produce much larger file sizes — do not use for submission

## Uploading the Build

Specify the following 4 files from the build folder in the unityroom submission form:
1. Loader file (`.js`)
2. Data file (`.data.gz`)
3. Framework file (`.framework.js.gz`)
4. Code file (`.wasm.gz`)

Be careful not to mix up the files.
