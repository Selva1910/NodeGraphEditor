# Changelog

All notable changes to this project will be documented in this file.

This format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

---

## [1.0.9] - 2026-02-22

### Changed
- **Unified release automation** — consolidated the GitHub Actions publish workflow into a single unified pipeline that triggers on `package.json` changes on `main`
- Workflow now runs `npm pack` to generate a `.tgz` tarball and attaches it to the GitHub Release automatically
- Release notes template updated to include both Git URL and tarball installation instructions

---

## [1.0.7] / [1.0.8] - 2026-02-22

> **Note:** v1.0.7 and v1.0.8 point to the same commit.

### Added
- **About window** (`NodeGraphEditorAbout`) — displays package version, author, and AGPL-3.0 license notice via `EditorUtility.DisplayDialog`
- **"Node Graph → About" menu item** in the Unity Editor top menu bar

### Removed
- **Scene Creation menu item** temporarily disabled (`//[MenuItem]`) until the feature is complete

### Changed
- `MenuManager` updated to expose only the `About` menu item; `New Sequence` is commented out pending Scene Creation feature completion
- GitHub Actions publish workflow further refined

---

## [1.0.6] - 2026-02-22

### Added
- **NOTICE file** — legal attribution notice for the AGPL-3.0 license, listing the original author and credit requirements for modifications
- **GitHub Actions publish workflow** (`PublicPackage.yml`) — automated tagging and release on version bump

### Removed
- Deleted `hello.yml` test workflow file

---

## [1.0.5] - 2026-02-22

### Added
- `CHANGELOG.md` — this file, following the Keep a Changelog standard
- `changelogUrl` and `licensesUrl` fields populated in `package.json`, linking to the GitHub-hosted files

### Changed
- `package.json` `displayName` updated to `"Node Graph Editor"`
- `package.json` `description` updated with a more complete project description
- `README.md` fully rewritten with professional GitHub standards: badges, feature list, architecture tree, custom node guide, keyboard shortcuts, and installation instructions

---

## [1.0.4] - 2026-02-22

### Changed
- `package.json` version bump and workflow adjustments

---

## [1.0.3] - 2026-02-22

### Changed
- GitHub Actions publish workflow updated (`PublicPackage.yml`)

---

## [1.0.2] - 2026-02-22

### Added
- **Copy / Paste support** — duplicate selected nodes with `Ctrl/Cmd+C` and `Ctrl/Cmd+V`; pasted nodes are offset by (50, 50) and auto-selected
- **Clipboard connection recreation** — connections between pasted nodes are automatically recreated when both endpoints were copied
- **Context menu** — right-click canvas shows Copy, Paste, and Delete actions for selected nodes
- **Frame All (`F` key)** — press `F` to fit all nodes into view
- **Mouse position tracking** — last mouse position tracked for paste placement
- `SceneObjectPickerDrawer` and `SceneObjectPropertyDrawer` — custom property drawers for in-scene object references
- `SceneObjectPickerAttribute` — attribute for annotating fields that reference scene objects
- `FloatValueNode` — lightweight node for passing float values through the graph
- `TestEventManagerNode`, `OnTestNode` — internal test nodes for event manager validation

### Fixed
- Node deletion now clears `NodeInspector` before removal, preventing `ObjectDisposedException`
- `DeepCopyNode` generates a fresh GUID for each copied node
- `DisconnectExistingOutputConnection` removes stale edges from both view and dictionary before reconnecting

### Changed
- `GraphNodeEditor.SerializedProperty` now always fetched dynamically by GUID, eliminating stale references after undo/redo

---

## [1.0.1] - 2026-02-22

### Added
- **Undo / Redo integration** — all graph mutations (add, move, delete, connect) wrapped with `Undo.RecordObject`
- **Node Inspector panel** (`NodeInspector`) — selecting a node opens a dedicated inspector panel
- **Parallel node** (`ParallelNode`) — branching node with configurable output count via `numberOfOutputs`
- `WaitForSecondsNode` — coroutine-based delay node for sequential execution
- `TimerNode` — repeating timer node driven by `OnFrameUpdate`
- `OnCollisionEnterNode`, `OnTriggerEnterNode` — physics event nodes
- `OnMouseClickNode` — mouse button event node with configurable button index
- `InputEventManager` — bridges Unity input events into the graph event system
- `ExposedInNodeAttribute`, `ExposedOutNodeAttribute` — render exposed fields as dedicated input/output ports

### Fixed
- Port connections now validate index bounds before drawing, preventing silent index-out-of-range errors
- `OnGraphViewChangedEvent` applies serialized property changes in a single batch after all removals

---

## [1.0.0] - 2025-10-01

### Added
- Initial release of **Node Graph Editor** (`com.cjhawk.graphnodeeditor`)
- `BaseGraphView` — core `GraphView` canvas with zoom, pan, grid, and manipulators
- `GraphAssetSO` — `ScriptableObject`-based graph data model (nodes + connections)
- `BaseGraphNode` — extensible base class with GUID, position, and type name
- `GraphConnection` / `GraphConnectionPort` — serializable connection model
- `INode` and `INodeLifeCycle` interfaces
- `GraphNodeEditor` — visual node element with automatic port and property rendering
- `GraphEditorWindow` — `EditorWindow` host; opens on double-clicking a `GraphAssetSO`
- `GraphAssetEditor` — custom Inspector with "Open Graph Editor" button
- `GraphWindowSearchProvider` — searchable node creation popup
- `NodeInfoAttribute`, `ExposedPropertyAttribute` — node and field annotations
- `EventGraphViewController` — event-driven runtime controller
- `LinearGraphViewController` — sequential runtime controller
- `EventManager` — central graph event dispatcher
- Built-in nodes: `EntryNode`, `DebugLogNode`, `MoveNode`, `RotateNode`, `OnEnableNode`, `OnFrameUpdateNode`, `OnKeyPressNode`
- `SceneObject` / `SceneObjectManager` — serializable in-scene object references
- USS stylesheet for graph canvas theming
- Assembly definitions: `NodeGraph.asmdef`, `NodeGraph.Editor.asmdef`

---

[Unreleased]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.9...HEAD
[1.0.9]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.8...v1.0.9
[1.0.8]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.7...v1.0.8
[1.0.7]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.6...v1.0.7
[1.0.6]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.5...v1.0.6
[1.0.5]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.4...v1.0.5
[1.0.4]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.3...v1.0.4
[1.0.3]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.2...v1.0.3
[1.0.2]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.1...v1.0.2
[1.0.1]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Selva1910/NodeGraphEditor/releases/tag/v1.0.0
