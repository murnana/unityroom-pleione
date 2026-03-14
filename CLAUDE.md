# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 6000.3.10f1 (LTS) template for publishing games to unityroom, a Japanese WebGL game hosting platform (<https://unityroom.com/>). Company: Murnana.

> **Status: Work In Progress (作成中)** — このテンプレートは現在開発中です。

- Render pipeline: Universal Render Pipeline (URP) 17.3.0, configured for 2D
- Target platform: WebGL (build output in `Builds/Release/`)
- Input: Modern Unity Input System (not the legacy Input Manager)
- Asset loading: Addressables 2.9.1
- Main scene: `Assets/Murnana UnityRoom/Samples/Sample.unity`

## Build

Builds are done through the Unity Editor (no CLI build scripts exist). The configured build profile is at `Assets/Settings/Build Profiles/Web - Release.asset`, targeting WebGL.

## Code Style (.editorconfig enforced)

- Indentation: 4 spaces
- Explicit typing — do not use `var`
- Namespaces: block-scoped (`namespace Foo { }` not file-scoped)
- Braces always required (no brace omission)
- Modifier order: public/private/protected/internal → static → readonly/const → ...

## Key Paths

| Path | Purpose |
|------|---------|
| `Assets/Murnana UnityRoom/Samples/` | Main scene and sample assets |
| `Assets/Settings/Inputs/` | Input System configuration and actions |
| `Assets/Settings/Renderings/` | URP renderer and volume profile assets |
| `Assets/Settings/Build Profiles/` | WebGL build profile |
| `Builds/Release/` | WebGL build output |
| `Packages/manifest.json` | UPM package dependencies |
| `THIRD PARTY NOTICES.md` | Third-party asset attribution (update when adding external assets) |

## Important Packages

- `com.unity.inputsystem` 1.18.0 — use `InputSystem_Actions.inputactions` for input bindings
- `com.unity.addressables` 2.9.1 — use for runtime asset loading
- `com.unity.render-pipelines.universal` 17.3.0 — URP 2D renderer
- `com.unity.2d.sprite` — 2D sprite support
