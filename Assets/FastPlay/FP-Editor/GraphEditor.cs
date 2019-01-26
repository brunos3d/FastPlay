#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using FastPlay.Runtime;
using System.Collections.Generic;

namespace FastPlay.Editor {
	public static class GraphEditor {

		private const string PREFS_MAKE_ALL_NODES_ACTIVE = "FastPlay: MakeAllNodesActive";

		private const string PREFS_SHOW_PORT_VALUES = "FastPlay: ShowPortValues";

		private const string PREFS_SNAP_TO_GRID = "FastPlay: SnapToGrid";

		private const string PREFS_CONNECTOR_TYPE = "FastPlay: ConnectorType";

		public static GraphAsset asset;

		public static Graph graph;

		public static float zoom = 1.0f;

		public static Rect window;

		public static Vector2 scroll = Vector2.zero;

		public static Vector2 mouse_position = Vector2.zero;

		public static Vector2 mouse_down_position = Vector2.zero;

		public static Node drag_node;

		public static Node hover_node;

		public static Port drag_port;

		public static Port hover_port;

		public static bool current_validate;

		public static bool waiting_for_new_node;

		public static bool can_select;

		public static bool is_select;

		public static bool is_scrolling;

		public static bool is_drag;

		public static bool can_drag_node;

		public static bool can_drag_port;

		public static List<Node> selection = new List<Node>();

		public static bool makeAllNodesActive {
			get {
				return EditorPrefs.GetBool(PREFS_MAKE_ALL_NODES_ACTIVE, false);
			}
			set {
				EditorPrefs.SetBool(PREFS_MAKE_ALL_NODES_ACTIVE, value);
			}
		}

		public static bool showPortValues {
			get {
				return EditorPrefs.GetBool(PREFS_SHOW_PORT_VALUES, true);
			}
			set {
				EditorPrefs.SetBool(PREFS_SHOW_PORT_VALUES, value);
			}
		}


		public static bool snapToGrid {
			get {
				return EditorPrefs.GetBool(PREFS_SNAP_TO_GRID, false);
			}
			set {
				EditorPrefs.SetBool(PREFS_SNAP_TO_GRID, value);
			}
		}

		public static ConnectorType connectorType {
			get {
				return (ConnectorType)EditorPrefs.GetInt(PREFS_CONNECTOR_TYPE, 0);
			}
			set {
				EditorPrefs.SetInt(PREFS_CONNECTOR_TYPE, (int)value);
			}
		}

		public static Node activeNode {
			get {
				if (selection.Count >= 1) {
					return selection[0];
				}
				return null;
			}
		}

		public static Rect ZoomedRect(Rect rect) {
			return new Rect(rect.position * zoom, rect.size * zoom);
		}

		public static Rect UnZoomedRect(Rect rect) {
			return new Rect(rect.position / zoom, rect.size / zoom);
		}

		public static void DuplicateSelectedNodes() {
			UndoManager.RecordObject(asset, string.Format("{0} Duplicate Nodes", asset.name));
			if (selection.Count > 0) {
				List<Node> new_selection = new List<Node>();
				foreach (Node node in selection) {
					Node clone = node.GetClone();
					new_selection.Add(graph.AddNode(clone, node.position + new Vector2(20.0f, 20.0f)));
					clone.Validate();
				}
				ClearSelection();
				SelectNodes(new_selection);
			}
			UndoManager.SetDirty(asset);
		}

		public static void DeleteSelectedNodes() {
			UndoManager.RecordObject(asset, string.Format("{0} Destroy Nodes", asset.name));
			if (selection.Count > 0) {
				graph.RemoveNodes(selection);
				ClearSelection();
			}
			UndoManager.SetDirty(asset);
		}

		public static void DeleteNode(Node node) {
			UndoManager.RecordObject(asset, string.Format("{0} > {1} Destroy Node", asset.name, node.name));
			graph.RemoveNode(node);
			UndoManager.SetDirty(asset);
		}

		public static void PlugPort(Port a, Port b) {
			if (a is IPlugIn && b is IPlugOut) {
				UndoManager.RecordObject(asset, string.Format("{0} > {1}: {2} Plug to {3}: {4}", asset.name, a.node.name, a.name, b.node.name, b.name));
				((IPlugIn)a).PlugTo((IPlugOut)b);
				UndoManager.SetDirty(asset);
			}
			else if (a is IPlugOut && b is IPlugIn) {
				UndoManager.RecordObject(asset, string.Format("{0} > {1}: {3} Plug to {2}: {4}", asset.name, a.node.name, a.name, b.node.name, b.name));
				((IPlugIn)b).PlugTo((IPlugOut)a);
				UndoManager.SetDirty(asset);
			}
		}

		public static void QuickPlugPort(Port port, Node node) {
			UndoManager.RecordObject(asset, string.Format("{0} > {1}: {2} Plug", asset.name, node.name, port.name));
			foreach (Port p in node.portValues) {
				if (p is IPlugOut && port is IPlugIn) {
					if (((IPlugIn)port).CanPlug((IPlugOut)p, false)) {
						((IPlugIn)port).PlugTo((IPlugOut)p);
						break;
					}
				}
				else if (p is IPlugIn && port is IPlugOut) {
					if (((IPlugIn)p).CanPlug((IPlugOut)port, false)) {
						((IPlugIn)p).PlugTo((IPlugOut)port);
						break;
					}
				}
			}
			UndoManager.SetDirty(asset);
		}

		public static void UnplugPort(Port port) {
			UndoManager.RecordObject(asset, string.Format("{0} > {1}: {2} Unplug", asset.name, port.node.name, port.name));
			if (port is IPlug) {
				((IPlug)port).Unplug();
			}
			UndoManager.SetDirty(asset);
		}

		public static void ClearSelection() {
			foreach (Node n in selection) {
				n.is_selected = false;
			}
			selection = new List<Node>();
			EditorUtils.RepaintInspector();
		}

		public static void SelectNode(int id, bool move = true) {
			SelectNode((Node)ObjectBase.InstanceIDToObject(id), move);
		}

		public static void SelectNode(Node node, bool move = true) {
			node.is_selected = true;
			if (!selection.Contains(node)) {
				selection.Add(node);
			}
			if (move) {
				graph.nodes = graph.nodes.MoveItemToEnd(node);
			}
			Selection.activeObject = asset;
			EditorUtils.RepaintInspector();
		}

		public static void SelectOnlyNode(int id, bool move = true) {
			SelectOnlyNode((Node)ObjectBase.InstanceIDToObject(id), move);
		}

		public static void SelectOnlyNode(Node node, bool move = true) {
			foreach (Node n in GraphEditor.selection) {
				n.is_selected = false;
			}
			node.is_selected = true;
			selection = new List<Node>() { node };
			if (move) {
				graph.nodes = graph.nodes.MoveItemToEnd(node);
			}
			Selection.activeObject = asset;
			EditorUtils.RepaintInspector();
		}

		public static void DeselectNode(int id, bool move = true) {
			DeselectNode((Node)ObjectBase.InstanceIDToObject(id), move);
		}

		public static void DeselectNode(Node node, bool move = true) {
			node.is_selected = false;
			selection.Remove(node);
			if (move) {
				graph.nodes = graph.nodes.MoveItemToStart(node);
			}
			Selection.activeObject = asset;
			EditorUtils.RepaintInspector();
		}

		public static void DeselectNodes(List<int> ids, bool move = true) {
			List<Node> nodes = new List<Node>();
			foreach (int id in ids) {
				nodes.Add((Node)ObjectBase.InstanceIDToObject(id));
			}
			DeselectNodes(nodes, move);
		}

		public static void DeselectNodes(List<Node> nodes, bool move = true) {
			List<Node> local_nodes = new List<Node>(nodes);
			foreach (Node node in local_nodes) {
				node.is_selected = false;
				selection.Remove(node);
			}
			if (move) {
				foreach (Node node in local_nodes) {
					graph.nodes = graph.nodes.MoveItemToStart(node);
				}
			}
			Selection.activeObject = asset;
			EditorUtils.RepaintInspector();
		}

		public static void SelectNodes(List<int> ids, bool move = true) {
			List<Node> nodes = new List<Node>();
			foreach (int id in ids) {
				nodes.Add((Node)ObjectBase.InstanceIDToObject(id));
			}
			SelectNodes(nodes, move);
		}

		public static void SelectNodes(List<Node> nodes, bool move = true) {
			List<Node> local_nodes = new List<Node>(nodes);
			foreach (Node node in local_nodes) {
				node.is_selected = true;
				if (!selection.Contains(node)) {
					selection.Add(node);
				}
			}
			if (move) {
				foreach (Node node in local_nodes) {
					graph.nodes = graph.nodes.MoveItemToEnd(node);
				}
			}
			Selection.activeObject = asset;
			EditorUtils.RepaintInspector();
		}
	}
}
#endif
