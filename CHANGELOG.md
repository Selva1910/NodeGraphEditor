# Changelog

All notable changes to this project will be documented in this file.

This format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.2] - 2026-02-22

### Added
- **Copy / Paste support** — duplicate selected nodes with `Ctrl/Cmd+C` and `Ctrl/Cmd+V`, pasted nodes are offset and auto-selected
- **Clipboard connection recreation** — connections between pasted nodes are automatically recreated when both endpoints were copied
- **Context menu actions** — right-click canvas shows Copy, Paste, and Delete actions for selected nodes
- **Frame All (`F` key)** — press `F` to fit all nodes into view using the built-in `FrameSelection` method
- **Mouse position tracking** — paste operations place nodes relative to the last known mouse position
- `SceneObjectPickerDrawer` and `SceneObjectPropertyDrawer` — custom property drawers for in-scene object references
- `SceneObjectPickerAttribute` — new attribute to annotate fields that reference scene objects
- `FloatValueNode` — lightweight node for passing float values through the graph
- `TestEventManagerNode`, `OnTestNode` — internal test nodes for event manager validation

### Fixed
- Node deletion now correctly clears the `NodeInspector` before removal, preventing `ObjectDisposedException`
- `DeepCopyNode` correctly skips the `m_guid` field and generates a fresh GUID for each copy
- `DisconnectExistingOutputConnection` properly removes stale edges from both the view and the connection dictionary before creating a new connection on the same output port

### Changed
- `GraphNodeEditor.SerializedProperty` is now always fetched dynamically by GUID, eliminating stale property references after undo/redo

---

## [1.0.1] - 2025-12-01

### Added
- **Undo / Redo integration** — all graph mutations (add, move, delete, connect) are wrapped with `Undo.RecordObject`
- **Node Inspector panel** (`NodeInspector`) — selecting a node opens a dedicated inspector showing its serialized properties
- **Parallel node** (`ParallelNode`) — branching node with configurable output count via `numberOfOutputs`
- `WaitForSecondsNode` — coroutine-based delay node for sequential graph execution
- `TimerNode` — repeating timer node driven by `OnFrameUpdate`
- `OnCollisionEnterNode`, `OnTriggerEnterNode` — physic event nodes
- `OnMouseClickNode` — mouse button event node with configurable button index
- `InputEventManager` — bridges Unity input events into the graph event system
- `ExposedInNodeAttribute`, `ExposedOutNodeAttribute` — render exposed fields as dedicated input/output ports on the node body

### Fixed
- Port connections now validate index bounds before attempting to draw, preventing silent `IndexOutOfRange` errors
- `OnGraphViewChangedEvent` now applies serialized property changes after processing all removals in a single batch

---

## [1.0.0] - 2025-10-01

### Added
- Initial release of **Node Graph Editor** (`com.cjhawk.graphnodeeditor`)
- `BaseGraphView` — core `GraphView` canvas with zoom, pan, grid background, and manipulators (drag, select, rectangle select)
- `GraphAssetSO` — `ScriptableObject`-based graph data model storing nodes and connections
- `BaseGraphNode` — extensible base class for all node types with GUID, position, and type name
- `GraphConnection` / `GraphConnectionPort` — serializable connection model between node ports
- `INode` interface — minimal contract for graph nodes
- `INodeLifeCycle` interface — `OnNodeEnter` / `OnNodeExit` lifecycle hooks
- `GraphNodeEditor` — visual `Node` element with automatic port and property rendering
- `GraphEditorWindow` — `EditorWindow` host; opens on double-clicking a `GraphAssetSO` asset
- `GraphAssetEditor` — custom Inspector for `GraphAssetSO` with an "Open Graph Editor" button
- `GraphWindowSearchProvider` — searchable node creation popup (Space / right-click)
- `NodeInfoAttribute` — `[NodeInfo]` annotation to register node title, menu path, and flow port configuration
- `ExposedPropertyAttribute` — `[ExposedProperty]` to surface fields inside the node body
- `EventGraphViewController` — runtime controller for event-driven graph execution
- `LinearGraphViewController` — runtime controller for sequential node execution
- `EventManager` — central dispatcher coordinating graph event nodes
- `EntryNode` — mandatory graph start node
- `DebugLogNode` — logs a string message to the Unity Console
- `MoveNode` — translates a target `GameObject` over time
- `RotateNode` — rotates a target `GameObject` over time
- `OnEnableNode` — fires on `MonoBehaviour.OnEnable`
- `OnFrameUpdateNode` — fires every frame via `Update`
- `OnKeyPressNode` — fires on a configurable key press
- `SceneObject` / `SceneObjectManager` — serializable wrapper and registry for in-scene `GameObject` references
- USS stylesheet (`GraphEditor.uss`) for graph canvas theming
- Assembly definitions: `NodeGraph.asmdef` (Runtime), `NodeGraph.Editor.asmdef` (Editor)

[1.0.2]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.1...v1.0.2
[1.0.1]: https://github.com/Selva1910/NodeGraphEditor/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Selva1910/NodeGraphEditor/releases/tag/v1.0.0
