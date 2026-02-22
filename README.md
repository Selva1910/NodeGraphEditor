# Node Graph Editor

> A visual node graph editor for Unity, built from scratch using the **GraphView** API — designed to power event-driven, flow-based gameplay logic through a ScriptableObject architecture.

[![Unity Version](https://img.shields.io/badge/Unity-2022.3%2B-blue?logo=unity)](https://unity.com)
[![Version](https://img.shields.io/badge/Package-v1.0.2-green)](https://github.com/Selva1910/NodeGraphEditor)
[![License](https://img.shields.io/badge/License-AGPL%20v3-red)](LICENSE)

---

## Overview

**Node Graph Editor** (`com.cjhawk.graphnodeeditor`) is a Unity Editor package that provides a fully functional, extensible visual node graph system. Graphs are stored as **ScriptableObject assets**, and the editor is powered by Unity's `GraphView` API.

It supports two execution models out of the box:
- **Event-driven execution** — nodes fire in response to Unity events (collision, key press, mouse click, etc.)
- **Linear sequential execution** — nodes execute one after another in a defined flow

---

## Features

- ✅ **Visual Node Graph Editor** — drag-and-drop canvas with zoom, pan, and grid background
- ✅ **ScriptableObject-Based Graphs** — graphs persist as Unity assets (`GraphAssetSO`)
- ✅ **Search Window** — quickly add nodes by name via a searchable popup
- ✅ **Node Inspector** — inspect and edit node properties in a dedicated panel when a node is selected
- ✅ **Copy / Paste** — duplicate nodes with `Ctrl/Cmd+C` and `Ctrl/Cmd+V`, with offset positioning
- ✅ **Undo / Redo** — full Unity Undo integration for all graph mutations
- ✅ **Delete** — remove selected nodes and their connections via `Delete` key or context menu
- ✅ **Frame All** — press `F` to fit all nodes in view
- ✅ **Parallel Execution** — `ParallelNode` with configurable output count for branching logic
- ✅ **Custom Node Attributes** — annotate fields with `[ExposedProperty]`, `[NodeInfo]`, `[DisplayName]`, and more
- ✅ **Event Nodes** — built-in nodes for `OnEnable`, `OnFrameUpdate`, `OnKeyPress`, `OnMouseClick`, `OnCollisionEnter`, `OnTriggerEnter`
- ✅ **Action Nodes** — built-in nodes for `Move`, `Rotate`, `WaitForSeconds`, `Timer`, `DebugLog`
- ✅ **Scene Object Support** — reference in-scene GameObjects from graph assets via `SceneObject`

---

## Requirements

| Requirement | Version |
|---|---|
| Unity | 2022.3.21f1 or later |
| Render Pipeline | Any (URP, HDRP, Built-in) |

---

## Installation

### Via Unity Package Manager (Git URL)

1. Open **Unity** → **Window** → **Package Manager**
2. Click the **`+`** button → **Add package from git URL**
3. Enter:
   ```
   https://github.com/Selva1910/NodeGraphEditor.git
   ```

### Manual Installation

1. Clone or download this repository
2. Copy the package folder into your project's `Packages/` directory
3. Unity will automatically detect and import it

---

## Getting Started

### 1. Create a Graph Asset

In the **Project** window, right-click and navigate to:

```
NodeGraph → Asset
```

This creates a new `GraphAssetSO` file in your project.

### 2. Open the Graph Editor

Double-click any `GraphAssetSO` asset to open the **Node Graph Editor** window.

### 3. Add Nodes

- **Right-click** on the canvas or **press Space** to open the node search window
- Select a node type to add it to the graph

### 4. Connect Nodes

- Drag from an **Out** port to an **In** port on another node to create a connection
- Connections only form between compatible port types

### 5. Run the Graph at Runtime

Attach an `EventGraphViewController` or `LinearGraphViewController` component to a GameObject and assign your `GraphAssetSO` to drive runtime execution.

---

## Architecture

```
NodeGraphEditor/
├── Editor/                         # Unity Editor-only code
│   ├── BaseGraphView.cs            # Core GraphView canvas (zoom, pan, CRUD, clipboard)
│   ├── GraphNodeEditor.cs          # Visual node element with port rendering
│   ├── GraphEditorWindow.cs        # EditorWindow host for the graph canvas
│   ├── GraphAssetEditor.cs         # Custom Inspector for GraphAssetSO
│   ├── GraphWindowSearchProvider.cs# Searchable node creation popup
│   ├── NodeInspector.cs            # Per-node property inspector panel
│   ├── MenuManager.cs              # Editor menu items
│   ├── PortTypes.cs                # Port type definitions
│   ├── SceneObjectPickerDrawer.cs  # Custom property drawer for SceneObject
│   ├── SceneObjectPropertyDrawer.cs
│   ├── Resources/                  # Editor resources (icons, USS styles)
│   └── USS/                        # Unity Style Sheets for graph theming
│
└── Runtime/                        # Runtime execution code
    ├── GraphAssetSO.cs             # ScriptableObject graph data model
    ├── BaseGraphNode.cs            # Base class for all nodes
    ├── GraphConnection.cs          # Port connection data model
    ├── INode.cs                    # Node interface
    ├── INodeLifeCycle.cs           # Lifecycle hooks (OnNodeEnter, OnNodeExit)
    ├── EventGraphViewController.cs # Event-driven graph execution controller
    ├── LinearGraphViewController.cs# Sequential graph execution controller
    ├── EventManager.cs             # Central graph event dispatcher
    ├── InputEventManager.cs        # Input event bridging for graphs
    ├── SceneObject.cs              # Serializable in-scene object reference
    ├── SceneObjectManager.cs       # Scene object registration & lookup
    ├── Attributes/                 # Custom C# attributes for node authoring
    │   ├── NodeInfoAttribute.cs    # [NodeInfo] — title, menu path, port config
    │   ├── ExposedPropertyAttribute.cs # [ExposedProperty] — show field in node
    │   ├── ExposedInNodeAttribute.cs   # [ExposedInNode] — render as input port
    │   └── SceneObjectPickerAttribute.cs
    ├── Properties/                 # Serializable property wrapper types
    └── Types/                      # Built-in node implementations
        ├── EntryNode.cs
        ├── ParallelNode.cs
        ├── TimerNode.cs
        ├── WaitForSecondsNode.cs
        ├── MoveNode.cs
        ├── RotateNode.cs
        ├── DebugLogNode.cs
        ├── OnEnableNode.cs
        ├── OnFrameUpdateNode.cs
        ├── OnKeyPressNode.cs
        ├── OnMouseClickNode.cs
        ├── OnCollisionEnterNode.cs
        └── OnTriggerEnterNode.cs
```

---

## Creating Custom Nodes

Extend `BaseGraphNode` and annotate it with `[NodeInfo]` to register it in the editor.

```csharp
using NodeGraph;
using UnityEngine;

[NodeInfo(
    title: "My Custom Node",
    menuItem: "Custom/My Custom Node",
    hasFlowInputs: true,
    hasFlowOutputs: true
)]
public class MyCustomNode : BaseGraphNode, INodeLifeCycle
{
    [ExposedProperty]
    [DisplayName("Message")]
    public string message = "Hello!";

    public void OnNodeEnter()
    {
        Debug.Log(message);
    }

    public void OnNodeExit() { }
}
```

| Attribute | Description |
|---|---|
| `[NodeInfo]` | Registers title, search menu path, and flow port configuration |
| `[ExposedProperty]` | Exposes a field in the node body and inspector |
| `[DisplayName]` | Overrides the display label of a field |
| `[ExposedInNode]` | Renders an exposed field as a dedicated input port |
| `[ExposedOutNode]` | Renders an exposed field as a dedicated output port |
| `INodeLifeCycle` | Provides `OnNodeEnter()` and `OnNodeExit()` callbacks |

---

## Keyboard Shortcuts

| Shortcut | Action |
|---|---|
| `Space` / Right-click | Open node search window |
| `F` | Frame all nodes in view |
| `Ctrl/Cmd + C` | Copy selected nodes |
| `Ctrl/Cmd + V` | Paste copied nodes |
| `Delete` | Delete selected nodes and connections |

---

## Acknowledgements

This project was inspired by and built upon concepts from the following YouTube live series:

> 🎬 **[Building A Node Editor Tool In Unity — Graph View (Live Session)](https://www.youtube.com/live/uXxBXGI-05k?si=ZVrLW17QtFdRrFrS)**
>
> A huge thanks to the author of this live session for a clear, in-depth walkthrough of Unity's `GraphView` API. This series laid the conceptual foundation for the node graph architecture used in this package.

---

## License

This project is licensed under the **GNU Affero General Public License v3.0 (AGPL-3.0)** — see the [LICENSE](LICENSE) file for details.

Key implications of AGPL-3.0:
- You may use, modify, and distribute this software freely
- Any modified version **must also be released under AGPL-3.0**
- If you run a modified version over a network, you **must make the source code available** to users

---

## Author

**Selvaraj Balakrishnan**
- 📧 [bselva1910@gmail.com](mailto:bselva1910@gmail.com)
- 🌐 [selva1910.github.io](https://selva1910.github.io/)
- 🐙 [github.com/Selva1910](https://github.com/Selva1910)
