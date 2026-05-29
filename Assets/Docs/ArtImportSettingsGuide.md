# Art Import Settings Guide

## Menu

Run `Escape From Nightmare / Art Resources / Apply Sprite Import Settings To Visual Resources`.

## Applied Settings

- Texture Type: `Sprite`
- Sprite Mode: `Single`
- Alpha Is Transparency: enabled
- Mip Maps: disabled
- Filter Mode: `Bilinear`
- Pixels Per Unit: `100`
- Texture Compression: `Uncompressed`

## Recommended Max Size

| Resource Type | Max Size |
|---|---:|
| Backgrounds | 4096 |
| ClueImages / ExamineImages | 2048 |
| Items | 512 |
| Symbols | 512 |
| UI | 1024 |
| Ghost | 2048 |

## Notes

- The tool only affects supported image files under `Assets/Resources`.
- Supported extensions are `.png`, `.jpg`, `.jpeg`, `.psd`, and `.tga`.
- If no images exist yet, the report records that no files were processed.
