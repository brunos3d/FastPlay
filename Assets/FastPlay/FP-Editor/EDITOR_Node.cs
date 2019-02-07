#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FastPlay.Editor;
using System.Reflection;

namespace FastPlay.Runtime {
	// Editor Node 
	public abstract partial class Node {

		public class Styles {
			public readonly GUIStyle label;
			public readonly GUIStyle unplug_button;
			public readonly GUIStyle unit;

			public readonly GUIStyle head_node;
			public readonly GUIStyle body_node;
			public readonly GUIStyle body_back_node;

			public readonly GUIStyle title_head;
			public readonly GUIStyle subtitle_head;
			public readonly GUIStyle node_message;

			public readonly GUIStyle slim_node;
			public readonly GUIStyle event_node;
			public readonly GUIStyle action_node;
			public readonly GUIStyle value_node;
			public readonly GUIStyle other_node;
			public readonly GUIStyle icon_node;
			public readonly GUIStyle highlight_node;

			public readonly GUIStyle input_port;
			public readonly GUIStyle output_port;
			public readonly GUIStyle input_action;
			public readonly GUIStyle output_action;

			public readonly GUIStyle on_input_port;
			public readonly GUIStyle on_output_port;
			public readonly GUIStyle on_input_action;
			public readonly GUIStyle on_output_action;

			public readonly GUIStyle input_label;
			public readonly GUIStyle output_label;

			public readonly Texture info_icon;
			public readonly Texture warning_icon;
			public readonly Texture error_icon;

			public Styles() {
				info_icon = EditorUtils.FindAssetByName<Texture>("console.infoicon");
				warning_icon = EditorUtils.FindAssetByName<Texture>("console.warnicon");
				error_icon = EditorUtils.FindAssetByName<Texture>("console.erroricon");

				label = FPSkin.skin.label;
				unplug_button = FPSkin.unplugButton;
				unit = FPSkin.unit;

				head_node = FPSkin.headNode;
				body_node = FPSkin.bodyNode;
				body_back_node = FPSkin.bodyBackgroundNode;

				title_head = FPSkin.titleHead;
				subtitle_head = FPSkin.subtitleHead;
				node_message = FPSkin.GetStyle("Node Message");

				slim_node = FPSkin.slimNode;
				event_node = FPSkin.eventNode;
				action_node = FPSkin.actionNode;
				value_node = FPSkin.valueNode;
				other_node = FPSkin.otherNode;
				icon_node = FPSkin.iconNode;
				highlight_node = FPSkin.highlightNode;

				input_port = FPSkin.inputPort;
				output_port = FPSkin.outputPort;
				input_action = FPSkin.inputAction;
				output_action = FPSkin.outputAction;

				on_input_port = new GUIStyle(FPSkin.inputPort);
				on_output_port = new GUIStyle(FPSkin.outputPort);
				on_input_action = new GUIStyle(FPSkin.inputAction);
				on_output_action = new GUIStyle(FPSkin.outputAction);

				on_input_port.normal = FPSkin.inputPort.onNormal;
				on_output_port.normal = FPSkin.outputPort.onNormal;
				on_input_action.normal = FPSkin.inputAction.onNormal;
				on_output_action.normal = FPSkin.outputAction.onNormal;

				on_input_port.hover = FPSkin.inputPort.onHover;
				on_output_port.hover = FPSkin.outputPort.onHover;
				on_input_action.hover = FPSkin.inputAction.onHover;
				on_output_action.hover = FPSkin.outputAction.onHover;

				input_label = FPSkin.inputLabel;
				output_label = FPSkin.outputLabel;
			}
		}

		private static Styles m_styles;

		public static Styles styles {
			get {
				return m_styles ?? (m_styles = new Styles());
			}
		}

		public const float ICON_SIZE = 35.0f;

		public const float ICON_SIZE_OFFSET = 45.0f;

		public const float MIN_SIZE_X = 100.0f;

		public const float MIN_SIZE_Y = 60.0f;

		public static readonly Vector2 V2x0y2 = new Vector2(0.0f, 2.0f);

		public static readonly Vector2 V2x8y9 = new Vector2(8.0f, 9.0f);

		public static readonly Vector2 V2x16y16 = new Vector2(16.0f, 16.0f);

		public static readonly Vector2 PORT_SIZE = new Vector2(20.0f, 20.0f);

		public static readonly Color HIGHLIGHT_COLOR = new Color(24.0f / 255.0f, 110.0f / 255.0f, 228.0f / 255.0f, 0.5f);

		public static Color gui_color;

		public static Color gui_back_color;

		public static Color gui_content_color;

		private bool has_message;

		private GUIContent node_message;

		private Vector2 node_message_size = Vector2.zero;

		private bool invert_title;

		private bool has_subtitle;

		private Rect head_rect;

		private Rect body_rect;

		private Rect icon_rect;

		private Rect title_rect;

		private Rect subtitle_rect;

		public Vector2 position = Vector2.zero;

		[NonSerialized]
		public Vector2 size;

		[NonSerialized]
		public bool slim;

		[NonSerialized]
		public float head_height;

		[NonSerialized]
		public float body_height;

		[NonSerialized]
		public bool is_active;

		[NonSerialized]
		public bool is_ready;

		[NonSerialized]
		public bool is_selected;

		[NonSerialized]
		public bool is_occluded;

		public static readonly Dictionary<int, Rect> points = new Dictionary<int, Rect>();

		public static readonly Dictionary<int, Rect> rects = new Dictionary<int, Rect>();

		public static readonly Dictionary<int, Color> colors = new Dictionary<int, Color>();

		public Rect nodeRect {
			get {
				Rect r;
				if (rects.TryGetValue(this.id, out r)) {
					return r;
				}
				return rects[this.id] = new Rect(this.position, this.size);
			}
		}

		void DrawNode() {
			GUI.Box(body_rect, GUIContent.none, styles.body_node);
			GUI.backgroundColor = gui_back_color;
			GUI.Box(body_rect, GUIContent.none, styles.body_back_node);
			GUI.backgroundColor = node_color;
			GUI.Box(head_rect, GUIContent.none, styles.head_node);

			GUI.Label(icon_rect, icon, styles.title_head);

			if (node_color.grayscale > 0.51f) {
				GUI.contentColor = Color.black;
			}
			else {
				GUI.contentColor = Color.white;
			}
			GUI.Label(title_rect, title, styles.title_head);

			if (has_subtitle) {
				GUI.Label(subtitle_rect, subtitle, styles.subtitle_head);
			}
			GUI.contentColor = gui_content_color;
			GUI.backgroundColor = gui_back_color;

			if (has_message) {
				GUI.Box(new Rect(head_rect.x, head_rect.y - node_message_size.y - 5.0f, node_message_size.x, node_message_size.y), node_message, styles.node_message);
			}
		}

		void DrawSlimNode() {
			Rect title_rect = new Rect(nodeRect);
			title_rect.x += 5.0f;
			title_rect.y -= 2.0f;
			title_rect.width -= 10.0f;
			GUI.Box(nodeRect, GUIContent.none, styles.slim_node);
			GUI.Label(title_rect, new GUIContent(title, icon, subtitle), styles.title_head);
		}

		public void EDITOR_DrawNode() {
			if (!is_ready) return;
			gui_color = GUI.color;
			gui_back_color = GUI.backgroundColor;
			gui_content_color = GUI.contentColor;

			if (is_active || is_selected || GraphEditor.hover_node == this || (GraphEditor.drag_port && GraphEditor.drag_port.node == this)) {
			}
			else {
				GUI.color = new Color(gui_color.r, gui_color.g, gui_color.b, 0.5f);
			}

			// for better performance
			if (is_occluded) {
				foreach (Port port in portValues) {
					if (!port.display_port) continue;
					IPlugIn plug_in = port as IPlugIn;
					if (plug_in == null || !plug_in.IsPlugged()) continue;
					port.node = this;

					Vector2 start = GetPortPoint(port).center;
					Vector2 end = GetPortPoint((Port)plug_in.GetPluggedPort()).center;
					if (port is ActionPort) {
						end = GetPortPoint(port).center;
						start = GetPortPoint((Port)plug_in.GetPluggedPort()).center;
					}

					Node.DrawConnection(start, end, GetPortColor(port), false);
					if (Application.isPlaying) {
						if (port.flow_state == FlowState.Active) {
							port.unit_delta_size = 1.0f;
							port.flow_state = FlowState.Idle;
						}
						else {
							port.unit_delta_size = Mathf.MoveTowards(port.unit_delta_size, 0.0f, EditorTime.deltaTime / 2.0f);
						}

						GUI.backgroundColor = GetPortColor(port);
						float distance = FPMath.SnapValue(Vector3.Distance(start, end) / 100.0f, 1);
						for (int id = 0; id < Mathf.RoundToInt(distance); id++) {
							float t = 1.0f - (((EditorTime.time / distance) + (1.0f / distance) * id) % 1.0f);
							Vector2 unit_size = V2x16y16 * port.unit_delta_size;
							GUI.Box(new Rect(LerpUnit(start, end, t) - V2x0y2 - unit_size / 2.0f, unit_size), "", styles.unit);
						}
						GUI.backgroundColor = gui_back_color;
					}
					else {
						if (GUI.Button(new Rect(MiddleOfConnection(end, start) - V2x8y9, V2x16y16), "x", styles.unplug_button)) {
							GraphEditor.UnplugPort(port);
						}
					}
				}
			}
			else {
				if (is_selected) {
					GUI.Box(nodeRect, string.Empty, styles.highlight_node);
				}

				is_active = GraphEditor.makeAllNodesActive || this is EventNode || this is InputNode || this is OutputNode;
				//Draw Node with custom color
				GUI.backgroundColor = node_color;
				if (slim) {
					DrawSlimNode();
				}
				else {
					DrawNode();
				}
				GUI.backgroundColor = gui_back_color;

				// Color gui_color = GUI.color;
				foreach (Port input in inputValues) {
					input.node = this;
					if (!input.display_port) continue;
					IPlug plug = input as IPlug;
					IPlugIn plug_in = input as IPlugIn;
					bool on = plug != null && plug.IsPlugged();
					IInputValue input_value = input as IInputValue;
					Rect port_rect = GetPortPoint(input);

					if (plug_in != null) {
						if (plug_in.IsPlugged()) {
							Vector2 start = port_rect.center;
							Vector2 end = GetPortPoint((Port)plug_in.GetPluggedPort()).center;

							Node.DrawConnection(start, end, GetPortColor(input), false);

							if (Application.isPlaying) {
								if (input.flow_state == FlowState.Active) {
									input.unit_delta_size = 1.0f;
									input.flow_state = FlowState.Idle;
								}
								else {
									input.unit_delta_size = Mathf.MoveTowards(input.unit_delta_size, 0.0f, EditorTime.deltaTime / 2.0f);
								}

								GUI.backgroundColor = GetPortColor(input);
								float distance = FPMath.SnapValue(Vector3.Distance(start, end) / 100.0f, 1);
								for (int id = 0; id < Mathf.RoundToInt(distance); id++) {
									float t = 1.0f - (((EditorTime.time / distance) + (1.0f / distance) * id) % 1.0f);
									Vector2 unit_size = V2x16y16 * input.unit_delta_size;
									GUI.Box(new Rect(LerpUnit(start, end, t) - V2x0y2 - unit_size / 2.0f, unit_size), "", styles.unit);
								}
								GUI.backgroundColor = gui_back_color;
							}
							else {
								if (GUI.Button(new Rect(MiddleOfConnection(start, end) - V2x8y9, V2x16y16), "x", styles.unplug_button)) {
									GraphEditor.UnplugPort(input);
								}
							}
						}
						else if (input_value != null) {
							if (GraphEditor.showPortValues) {
								object value = input_value.GetDefaultValue();
								Rect value_label_rect;
								string value_content = "NO INFO";
								float value_label_width = 0.0f;
								if (value == null) {
									if (typeof(UnityEngine.Component).IsAssignableFrom(input_value.valueType) || typeof(Graph).IsAssignableFrom(input_value.valueType) || typeof(UnityEngine.GameObject).IsAssignableFrom(input_value.valueType)) {
										if (EditorGUIUtility.isProSkin) {
											value_content = string.Format("<b><color=#0667FF>SELF: {0}</color></b>", input_value.valueType.GetTypeName());
										}
										else {
											value_content = string.Format("<b><color=#458fff>SELF: {0}</color></b>", input_value.valueType.GetTypeName());
										}
									}
									else {
										value_content = input_value.valueType.GetTypeName(true);
									}

								}
								else {
									if (typeof(string).IsAssignableFrom(input_value.valueType)) {
										value_content = string.Format("<color=#FFA06396>\"{0}\"</color>", value);
									}
									else if (typeof(UnityEngine.Component).IsAssignableFrom(input_value.valueType) || typeof(UnityEngine.GameObject).IsAssignableFrom(input_value.valueType) || typeof(Graph).IsAssignableFrom(input_value.valueType)) {
										value_content = string.Format("<b><color=#0667FF>{0}</color></b>", value);
									}
									else if (typeof(Type).IsAssignableFrom(input_value.valueType)) {
										value_content = ReflectionUtils.GetTypeName((Type)value, true);
									}
									else {
										if (input_value.valueType.IsGenericType) {
											value_content = input_value.valueType.GetTypeName(true);
										}
										else {
											value_content = value.ToString();
										}
									}
								}
								value_label_width = GUIUtils.GetTextWidth(value_content, styles.input_label);
								value_label_rect = new Rect(port_rect.x - (value_label_width + 15.0f), port_rect.y, value_label_width, 18.0f);
								GUI.Label(value_label_rect, value_content, styles.input_label);
							}
						}
					}

					port_rect = GraphEditor.ZoomedRect(GetPortPoint(input));
					if (port_rect.Contains(GraphEditor.mouse_position)) {
						GraphEditor.hover_port = input;
					}
					else {
						GUI.backgroundColor = GetPortColor(input);
					}
					port_rect = GetPortPoint(input);

					if (input is ActionPort) {
						if (!is_active && ((IPlug)input).IsPlugged()) {
							List<IPlugIn> list = ((IPlugOut)input).GetPluggedPorts();
							if (list != null && list.Any(p => ((Port)p).node && ((Port)p).node.is_active)) {
								is_active = true;
							}
						}
						GUI.Box(port_rect, slim ? string.Empty : input.name, on ? styles.on_input_action : styles.input_action);
					}
					else {
						GUI.Box(port_rect, slim ? string.Empty : input.name, on ? styles.on_input_port : styles.input_port);
					}

					GUI.backgroundColor = gui_back_color;
				}

				foreach (Port output in outputValues) {
					output.node = this;
					if (!output.display_port) continue;
					IPlug plug = output as IPlug;
					IPlugIn plug_in = output as IPlugIn;
					bool on = plug != null && plug.IsPlugged();
					Rect port_rect = GetPortPoint(output);

					if (plug_in != null) {
						if (plug_in.IsPlugged()) {
							Vector2 start = port_rect.center;
							Vector2 end = GetPortPoint((Port)plug_in.GetPluggedPort()).center;

							Node.DrawConnection(end, start, GetPortColor(output), false);

							if (Application.isPlaying) {
								if (output.flow_state == FlowState.Active) {
									output.unit_delta_size = 1.0f;
									output.flow_state = FlowState.Idle;
								}
								else {
									output.unit_delta_size = Mathf.MoveTowards(output.unit_delta_size, 0.0f, EditorTime.deltaTime / 2.0f);
								}

								float distance = FPMath.SnapValue(Vector3.Distance(start, end) / 100.0f, 1);
								GUI.backgroundColor = GetPortColor(output);
								for (int id = 0; id < Mathf.RoundToInt(distance); id++) {
									float t = 1.0f - (((EditorTime.time / distance) + (1.0f / distance) * id) % 1.0f);
									Vector2 unit_size = V2x16y16 * output.unit_delta_size;
									GUI.Box(new Rect(LerpUnit(end, start, t) - V2x0y2 - unit_size / 2.0f, unit_size), "", styles.unit);
								}
								GUI.backgroundColor = gui_back_color;
							}
							else {
								if (GUI.Button(new Rect(MiddleOfConnection(start, end) - V2x8y9, V2x16y16), "x", styles.unplug_button)) {
									GraphEditor.UnplugPort(output);
								}
							}
						}
					}
					port_rect = GraphEditor.ZoomedRect(GetPortPoint(output));
					if (port_rect.Contains(GraphEditor.mouse_position)) {
						GraphEditor.hover_port = output;
					}
					else {
						GUI.backgroundColor = GetPortColor(output);
					}
					port_rect = GetPortPoint(output);

					if (output is ActionPort) {
						GUI.Box(port_rect, slim ? string.Empty : output.name, on ? styles.on_output_action : styles.output_action);
					}
					else {
						if (!is_active && ((IPlug)output).IsPlugged() && ((IPlugOut)output).GetPluggedPorts().Any(p => ((Port)p).node.is_active)) {
							is_active = true;
						}
						GUI.Box(port_rect, slim ? string.Empty : output.name, on ? styles.on_output_port : styles.output_port);
					}
					GUI.backgroundColor = gui_back_color;
				}
				GUI.backgroundColor = gui_back_color;
			}
			GUI.color = gui_color;
			GUI.backgroundColor = gui_back_color;
			GUI.contentColor = gui_content_color;

		}

		public void DisplayMessage(string message, MessageType type) {
			this.has_message = true;
			switch (type) {
				case MessageType.Warning:
					this.node_message = new GUIContent(message, styles.warning_icon);
					break;
				case MessageType.Error:
					this.node_message = new GUIContent(message, styles.error_icon);
					break;
				default:
					this.node_message = new GUIContent(message, styles.info_icon);
					break;
			}
			this.node_message_size = GUIUtils.GetTextSize(this.node_message, styles.node_message);
		}

		public void EDITOR_Update() {
			if (!is_ready) {
				EDITOR_Prepare();
				return;
			}
			if (this is IVariable && (this as IVariable).GetVariableName() != title) {
				EDITOR_Prepare();
				return;
			}
			if (GraphEditor.snapToGrid) {
				if (!GraphEditor.is_drag) {
					position = FPMath.SnapVector2(position);
				}
				rects[this.id] = new Rect(FPMath.SnapVector2(this.position) + GraphEditor.scroll, this.size);
			}
			else {
				rects[this.id] = new Rect(this.position + GraphEditor.scroll, this.size);
			}
			Vector2 pos = rects[this.id].position;
			head_rect.position = pos;
			body_rect.position = new Vector2(pos.x, pos.y + head_height);
			icon_rect.position = new Vector2(pos.x + 5.0f, pos.y + 5.0f);
			if (has_subtitle) {
				if (invert_title) {
					title_rect.position = new Vector2(pos.x + ICON_SIZE_OFFSET, pos.y + 23.0f);
					subtitle_rect.position = new Vector2(pos.x + ICON_SIZE_OFFSET, pos.y + 6.0f);
				}
				else {
					title_rect.position = new Vector2(pos.x + ICON_SIZE_OFFSET, pos.y + 6.0f);
					subtitle_rect.position = new Vector2(pos.x + ICON_SIZE_OFFSET, pos.y + 23.0f);
				}
			}
			else {
				title_rect.position = new Vector2(pos.x + ICON_SIZE_OFFSET, pos.y + 15.0f);
			}
		}

		// Prepare Node for editing
		public void EDITOR_Prepare() {
			RegisterPorts();

			GenerateBody();
			is_ready = true;
		}

		public void GenerateBody() {
			GenerateContent();
			if (slim) {
				head_height = 0.0f;
				body_height = 32.0f;
				size = new Vector2(GetWidth(), head_height + body_height);
			}
			else {
				head_height = ICON_SIZE_OFFSET;

				body_height = GetHeight();

				// Calculate Size...
				size = new Vector2(GetWidth(), head_height + body_height);
				// Recalculate Size...
				size = Vector2.Max(size, new Vector2(MIN_SIZE_X, MIN_SIZE_Y));
				// Create nodeRect...
			}
			if (GraphEditor.snapToGrid) {
				rects[this.id] = new Rect(FPMath.SnapVector2(this.position) + GraphEditor.scroll, this.size);
			}
			else {
				rects[this.id] = new Rect(this.position + GraphEditor.scroll, this.size);
			}
			// Recalculate Size...
			CreatePortPoints();
			GenerateRects();
		}

		public void GenerateRects() {
			head_rect = new Rect(position, new Vector2(size.x, head_height));
			body_rect = new Rect(position.x, position.y + head_height, size.x, body_height);
			subtitle_rect = new Rect(position.x + ICON_SIZE_OFFSET, position.y + 28.0f, size.x - ICON_SIZE_OFFSET, head_height - 28.0f);

			if (has_subtitle) {
				icon_rect = new Rect(position.x + 5.0f, position.y + 5.0f, ICON_SIZE, ICON_SIZE);
				if (this is EventNode) {
					title_rect = new Rect(position.x + ICON_SIZE_OFFSET, position.y + 6.0f, size.x - ICON_SIZE_OFFSET, 20.0f);
				}
				else {
					title_rect = new Rect(position.x + ICON_SIZE_OFFSET, position.y + 23.0f, size.x - ICON_SIZE_OFFSET, head_height - 23.0f);
					subtitle_rect = new Rect(position.x + ICON_SIZE_OFFSET, position.y + 8.0f, size.x - ICON_SIZE_OFFSET, head_height - 28.0f);
				}
			}
			else {
				icon_rect = new Rect(position.x + 5.0f, position.y + 5.0f, ICON_SIZE, ICON_SIZE);
				title_rect = new Rect(position.x + ICON_SIZE_OFFSET, position.y + 15.0f, size.x - ICON_SIZE_OFFSET, 20.0f);
			}
		}

		public void GenerateContent() {
			if (this is IValueNode) {
				node_color = GUIReferrer.GetTypeColor(((IValueNode)this).valueType);
			}
			else if (!((this is ReflectedNode))) {
				node_color = GUIReferrer.GetTypeColor(type);
			}
			if (this is IVariable) {
				this.title = (this as IVariable).GetVariableName();
			}

			TitleAttribute flag_title = type.GetAttribute<TitleAttribute>(false);
			if (flag_title != null) {
				this.title = flag_title.title;
			}
			SubtitleAttribute flag_subtitle = type.GetAttribute<SubtitleAttribute>(true);
			if (flag_subtitle != null) {
				this.subtitle = flag_subtitle.subtitle;
			}
			IconAttribute flag_icon = type.GetAttribute<IconAttribute>(false);
			if (flag_icon != null) {
				this.icon = flag_icon.GetIcon();
			}
			SlimAttribute flag_slim = type.GetAttribute<SlimAttribute>(false);
			this.slim = flag_slim != null && flag_slim.is_slim;

			if (this is EventNode) {
				if (this.subtitle.IsNullOrEmpty()) {
					this.subtitle = "Event";
				}
			}
			else if (this is ReflectedNode) {
				invert_title = true;
				MethodInfo method = ((ReflectedNode)this).cached_method;
				if (method != null) {
					Type reflected_type = ((ReflectedNode)this).cached_method.ReflectedType;
					if (this.subtitle.IsNullOrEmpty()) {
						this.subtitle = reflected_type.GetTypeName();
					}
					this.icon = this.icon ?? GUIReferrer.GetTypeIcon(reflected_type);
				}
				else {
					this.title = "Missing Method";
					this.icon = styles.error_icon;
				}
			}
			if (this is IValueNode) {
				this.icon = this.icon ?? GUIReferrer.GetTypeIcon(((IValueNode)this).valueType);
			}
			if (this.title.IsNullOrEmpty()) {
				if (this is IValueNode) {
					this.title = (this as IValueNode).valueType.GetTypeName(false, true);
				}
				else {
					this.title = this.name;
				}
			}
			this.icon = this.icon ?? GUIReferrer.GetTypeIcon(type);
			has_subtitle = !subtitle.IsNullOrEmpty();
		}

		public void CreatePortPoints() {
			int input = 0;
			int output = 0;
			Vector2 port_position = Vector2.zero;

			// IInputPort
			for (int id = 0; id < this.inputs.Count; id++) {
				Port port = this.inputValues[id];
				if (!port.display_port) continue;
				if (slim) {
					port_position = new Vector2(5.0f, 10.0f);
				}
				else {
					port_position = new Vector2(5.0f, head_height + 10.0f + (20.0f * input));
				}
				points[port.id] = new Rect(port_position, PORT_SIZE);
				if (port is ValuePort) {
					colors[port.id] = GUIReferrer.GetTypeColor((port as ValuePort).valueType);
				}
				else {
					colors[port.id] = GUIReferrer.GetTypeColor(port.type);
				}
				input++;
			}

			// IOutputPort  
			for (int id = 0; id < this.outputs.Count; id++) {
				Port port = this.outputValues[id];
				if (!port.display_port) continue;
				if (slim) {
					port_position = new Vector2(size.x - 23.0f, 10.0f);
				}
				else {
					port_position = new Vector2(size.x - 23.0f, head_height + 10.0f + (20.0f * output));
				}
				points[port.id] = new Rect(port_position, PORT_SIZE);
				if (port is ValuePort) {
					colors[port.id] = GUIReferrer.GetTypeColor((port as ValuePort).valueType);
				}
				else {
					colors[port.id] = GUIReferrer.GetTypeColor(port.type);
				}
				output++;
			}
		}

		public static Rect GetPortPoint(Port port) {
			Rect p;
			if (points.TryGetValue(port.id, out p)) {
				p.position += rects[port.node.id].position;
				return p;
			}
			return default(Rect);
		}

		public static Color GetPortColor(Port port) {
			Color c;
			if (colors.TryGetValue(port.id, out c)) {
				return c;
			}
			else {
				if (port is ValuePort) {
					return (colors[port.id] = GUIReferrer.GetTypeColor((port as ValuePort).valueType));
				}
				else {
					return (colors[port.id] = GUIReferrer.GetTypeColor(port.type));
				}
			}
		}

		public float GetWidth() {
			float width = MIN_SIZE_X;
			float in_width = 0.0f;
			float out_width = 0.0f;

			for (int id = 0; id < inputs.Count; id++) {
				string port_name = inputKeys[id];
				Port port = inputs[port_name];
				if (!port.display_port) continue;
				float widthtxt = 28.0f + GUIUtils.GetTextWidth(port_name, port is ActionPort ? styles.input_action : styles.input_port);

				if (widthtxt > in_width) {
					in_width = widthtxt;
				}
			}
			for (int id = 0; id < outputs.Count; id++) {
				string port_name = outputKeys[id];
				Port port = outputs[port_name];
				if (!port.display_port) continue;
				float widthtxt = 28.0f + GUIUtils.GetTextWidth(port_name, port is ActionPort ? styles.output_action : styles.output_port);
				if (widthtxt > out_width) {
					out_width = widthtxt;
				}
			}

			if (slim) {
				width = 70.0f + GUIUtils.GetTextWidth(this.title, styles.title_head);
			}
			else {
				width = in_width + out_width;
				float name_width = 50.0f + GUIUtils.GetTextWidth(this.title, styles.title_head);
				float subtitle_width = 50.0f + GUIUtils.GetTextWidth(this.subtitle, styles.subtitle_head);
				if (subtitle_width > 240.0f) {
					head_height += (FPMath.SnapValue(subtitle_width) / 240.0f) * 12.0f;
					head_height += this.subtitle.Split('\n').Length * 10.0f;
					subtitle_width = 240.0f;
				}

				width = Mathf.Max(name_width, subtitle_width, width) + 15.0f;
			}
			return width;
		}

		public float GetHeight() {
			int input = inputs.Count;
			int output = outputs.Count;
			int winner = 1;
			float height = 0.0f;

			winner = Mathf.Max(winner, input, output);
			height = winner * 20.0f;
			return height + 16.0f;
		}

		public static Vector2 MiddleOfConnection(Vector2 start, Vector2 end) {
			return FPMath.CenterOfPoints(start, end);
		}

		public static void DrawConnection(Vector2 start, Vector2 end, Color gizmoColor, bool selected) {
			float delta = Mathf.Abs(start.x - end.x) * 0.5f;
			Vector2 right = new Vector2(delta, 0.0f);
			Vector2 left = new Vector2(-delta, 0.0f);
			Vector2 p1 = new Vector2(start.x, start.y - 2.0f);
			Vector2 p2 = new Vector2(end.x, end.y - 2.0f);
			Vector2 t1 = p1 + left;
			Vector2 t2 = p2 + right;

			switch (GraphEditor.connectorType) {
				case ConnectorType.Bezier:
					if (Mathf.Abs(start.y - end.y) <= 1.0f) {
						goto case ConnectorType.Line;
					}

					if (selected) {
						// highlight
						Handles.color = new Color(24.0f / 255.0f, 110.0f / 255.0f, 228.0f / 255.0f, 0.5f);
						for (int id = 0; id <= 3; ++id) {
							DrawBezier(6.0f * id, p1, p2, t1, t2);
						}
					}
					else {
						// shadow
						Handles.color = new Color(0.0f, 0.0f, 0.0f, 0.75f);
						Vector2 st1 = (p1 + V2x0y2) + left;
						Vector2 st2 = (p2 + V2x0y2) + right;
						DrawBezier(5.0f, (p1 + V2x0y2), (p2 + V2x0y2), st1, st2);
					}
					Handles.color = gizmoColor;
					DrawBezier(3.0f, p1, p2, t1, t2);
					return;
				case ConnectorType.Line:
					if (selected) {
						// highlight
						Handles.color = HIGHLIGHT_COLOR;
						for (int id = 0; id <= 3; ++id) {
							Handles.DrawAAPolyLine(6.0f * id, 2, p1, p2);
						}
					}
					else {
						// shadow
						Handles.color = new Color(0.0f, 0.0f, 0.0f, 0.75f);
						for (int id = 0; id < 2; id++) {
							Handles.DrawAAPolyLine(5.0f, 2, p1 + V2x0y2, p2 + V2x0y2);
						}
					}
					Handles.color = gizmoColor;

					for (int id = 0; id < 2; id++) {
						Handles.DrawAAPolyLine(3.0f, 2, p1, p2);
					}
					return;
				case ConnectorType.Circuit:
					if (Mathf.Abs(start.y - end.y) <= 1.0f) {
						goto case ConnectorType.Line;
					}

					if (selected) {
						// highlight
						Handles.color = HIGHLIGHT_COLOR;

						for (int id = 0; id <= 3; ++id) {
							DrawCircuit(6.0f * id, p1, p2, t1, t2);
						}
					}
					else {
						// shadow
						Handles.color = new Color(0.0f, 0.0f, 0.0f, 0.75f);

						for (int id = 0; id < 3; id++) {
							DrawCircuit(5.0f, p1 + V2x0y2, p2 + V2x0y2, t1 + V2x0y2, t2 + V2x0y2);
						}
					}
					Handles.color = gizmoColor;
					DrawCircuit(3.0f, p1, p2, t1, t2);
					return;
				default:
					goto case ConnectorType.Line;
			}
		}

		private static void DrawBezier(float width, Vector2 p1, Vector2 p2, Vector2 t1, Vector2 t2) {
			bool inverted = p1.x < p2.x;
			if (inverted) {
				float center_y = FPMath.CenterOfPoints(p1, p2).y;
				Vector2 p1_center = new Vector2(p1.x, center_y);
				Vector2 p2_center = new Vector2(p2.x, center_y);

				float delta = Mathf.Abs(p1.x - p2.x);
				float tangent = Mathf.Min(Vector2.Distance(p1, p1_center) * 0.5f, delta);

				Vector2 p1_tan = new Vector2(p1.x - tangent, p1.y);
				Vector2 p1c_tan = new Vector2(p1_tan.x, center_y);

				Vector2 p2_tan = new Vector2(p2.x + tangent, p2.y);
				Vector2 p2c_tan = new Vector2(p2_tan.x, center_y);

				Handles.DrawAAPolyLine(width, 2, p1_center, p2_center);

				Handles.DrawBezier(p1, p1_center, p1_tan, p1c_tan, Handles.color, null, width);
				Handles.DrawBezier(p2, p2_center, p2_tan, p2c_tan, Handles.color, null, width);
			}
			else {
				Handles.DrawBezier(p1, p2, t1, t2, Handles.color, null, width);
			}
		}

		private static void DrawCircuit(float width, Vector2 p1, Vector2 p2, Vector2 t1, Vector2 t2) {
			bool inverted = p1.x < p2.x;
			if (inverted) {
				float center_y = FPMath.CenterOfPoints(p1, p2).y;
				float delta = Mathf.Min(Mathf.Abs(p1.x - p2.x) * 0.5f, 40.0f);
				Vector2 t1_pos = new Vector2(p1.x - delta, p1.y);
				Vector2 t2_pos = new Vector2(p2.x + delta, p2.y);
				Vector2 t1_center = new Vector2(t1_pos.x, center_y);
				Vector2 t2_center = new Vector2(t2_pos.x, center_y);

				Handles.DrawAAPolyLine(width, 6, p1, t1_pos, t1_center, t2_center, t2_pos, p2);
			}
			else {
				Handles.DrawAAPolyLine(width, 4, p1, t1, t2, p2);
			}
		}

		public static Vector2 LerpUnit(Vector2 start, Vector2 end, float t) {
			float delta = Mathf.Abs(start.x - end.x) * 0.5f;
			Vector2 right = new Vector2(delta, 0.0f);
			Vector2 left = new Vector2(-delta, 0.0f);
			Vector2 p1 = new Vector2(start.x, start.y);
			Vector2 p2 = new Vector2(end.x, end.y);
			Vector2 t1 = p1 + left;
			Vector2 t2 = p2 + right;

			bool inverted = p1.x < p2.x;
			float time = (t % 0.333333f) * 3.0f;
			switch (GraphEditor.connectorType) {
				case ConnectorType.Bezier:
					if (inverted) {
						float center_y = FPMath.CenterOfPoints(p1, p2).y;
						Vector2 p1_center = new Vector2(p1.x, center_y);
						Vector2 p2_center = new Vector2(p2.x, center_y);

						float tangent = Mathf.Min(Vector2.Distance(p1, p1_center) * 0.5f, delta);

						Vector2 p1_tan = new Vector2(p1.x - tangent, p1.y);
						Vector2 p1c_tan = new Vector2(p1_tan.x, center_y);

						Vector2 p2_tan = new Vector2(p2.x + tangent, p2.y);
						Vector2 p2c_tan = new Vector2(p2_tan.x, center_y);

						Vector2 l1 = Vector2.zero;
						Vector2 l2 = Vector2.zero;
						Vector2 l3 = Vector2.zero;

						Vector2 l4 = Vector2.zero;
						Vector2 l5 = Vector2.zero;

						if (t <= 0.333333f) {
							l1 = Vector2.Lerp(p1, p1_tan, time);
							l2 = Vector2.Lerp(p1_tan, p1c_tan, time);
							l3 = Vector2.Lerp(p1c_tan, p1_center, time);

							l4 = Vector2.Lerp(l1, l2, time);
							l5 = Vector2.Lerp(l2, l3, time);
						}
						else if (t <= 0.666666f) {
							return Vector2.Lerp(p1_center, p2_center, time);
						}
						else {
							l1 = Vector2.Lerp(p2_center, p2c_tan, time);
							l2 = Vector2.Lerp(p2c_tan, p2_tan, time);
							l3 = Vector2.Lerp(p2_tan, p2, time);

							l4 = Vector2.Lerp(l1, l2, time);
							l5 = Vector2.Lerp(l2, l3, time);
						}
						return Vector2.Lerp(l4, l5, time);
					}
					else {
						Vector2 l1 = Vector2.Lerp(p1, t1, t);
						Vector2 l2 = Vector2.Lerp(t1, t2, t);
						Vector2 l3 = Vector2.Lerp(t2, p2, t);

						Vector2 l4 = Vector2.Lerp(l1, l2, t);
						Vector2 l5 = Vector2.Lerp(l2, l3, t);

						return Vector2.Lerp(l4, l5, t);
					}
				case ConnectorType.Line:
					return Vector2.Lerp(p1, p2, t);
				case ConnectorType.Circuit:
					if (inverted) {
						time = (t % 0.2f) * 5.0f;
						float center_y = FPMath.CenterOfPoints(p1, p2).y;
						delta = Mathf.Min(Mathf.Abs(p1.x - p2.x) * 0.5f, 40.0f);
						Vector2 t1_pos = new Vector2(p1.x - delta, p1.y);
						Vector2 t2_pos = new Vector2(p2.x + delta, p2.y);
						Vector2 t1_center = new Vector2(t1_pos.x, center_y);
						Vector2 t2_center = new Vector2(t2_pos.x, center_y);
						//p1, t1_pos, t1_center, t2_center, t2_pos, p2
						if (t <= 0.2f) {
							return Vector2.Lerp(p1, t1_pos, time);
						}
						else if (t <= 0.4f) {
							return Vector2.Lerp(t1_pos, t1_center, time);
						}
						else if (t <= 0.6f) {
							return Vector2.Lerp(t1_center, t2_center, time);
						}
						else if (t <= 0.8f) {
							return Vector2.Lerp(t2_center, t2_pos, time);
						}
						else {
							return Vector2.Lerp(t2_pos, p2, time);
						}
					}
					else {
						if (t <= 0.333333f) {
							return Vector2.Lerp(p1, t1, time);
						}
						else if (t <= 0.666666f) {
							return Vector2.Lerp(t1, t2, time);
						}
						else {
							return Vector2.Lerp(t2, p2, time);
						}
					}
				default:
					goto case ConnectorType.Line;
			}
		}
	}
}
#endif
