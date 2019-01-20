using OdinSerializer;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[HideInList]
	public abstract partial class Node : ObjectBase {

		#region Editor

		[NonSerialized]
		public string title;

		[NonSerialized]
		public string subtitle;

		[NonSerialized]
		public Texture icon;

		[NonSerialized]
		public Color node_color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		#endregion

		public Graph graph;

		public MacroNode macro;

		public List<Node> nodes = new List<Node>();

		private List<string> port_keys;

		private List<string> input_keys;

		private List<string> output_keys;

		private List<Port> port_values;

		private List<Port> input_values;

		private List<Port> output_values;

		private List<string> recently_modified = new List<string>();

		public Dictionary<string, Port> ports = new Dictionary<string, Port>();

		public Dictionary<string, Port> inputs = new Dictionary<string, Port>();

		public Dictionary<string, Port> outputs = new Dictionary<string, Port>();

		public int nodeCount {
			get {
				return nodes.Count;
			}
		}

		public List<string> portKeys {
			get {
				return port_keys ?? (port_keys = ports.Keys.ToList());
			}
		}

		public List<Port> portValues {
			get {
				return port_values ?? (port_values = ports.Values.ToList());
			}
		}

		public List<string> inputKeys {
			get {
				return input_keys ?? (input_keys = inputs.Keys.ToList());
			}
		}

		public List<Port> inputValues {
			get {
				return input_values ?? (input_values = inputs.Values.ToList());
			}
		}

		public List<string> outputKeys {
			get {
				return output_keys ?? (output_keys = outputs.Keys.ToList());
			}
		}

		public List<Port> outputValues {
			get {
				return output_values ?? (output_values = outputs.Values.ToList());
			}
		}

		public Node() { }

		public virtual void Validate() {
#if UNITY_EDITOR
			EDITOR_Prepare();
#endif
		}

		public virtual void Initialize() {
			RegisterPorts();
			RegisterEvents();
			foreach (Port port in portValues) {
				port.Initialize();
			}
		}

		public virtual void Finish() {
			RemoveEvents();
			foreach (Port port in portValues) {
				port.Finish();
			}
		}

		public virtual void OnGraphAdd() { }

		public virtual void OnGraphRemove() { }

		public virtual void OnGraphPlay() { }

		public virtual void OnGraphStop() { }

		public void Call(ActionPort port) {
#if UNITY_EDITOR
			port.flow_state = FlowState.Active;
#endif
			port.Call();
		}

		public void Call(Act action) {
			action();
		}

		public void Call<T>(Act<T> action, T arg) {
			action(arg);
		}

		public void RegisterPorts() {
			recently_modified = new List<string>();

			ports = ports ?? new Dictionary<string, Port>();
			inputs = inputs ?? new Dictionary<string, Port>();
			outputs = outputs ?? new Dictionary<string, Port>();

			List<string> old_port_keys = new List<string>(ports.Keys);
			List<string> old_inputs_keys = new List<string>(inputs.Keys);
			List<string> old_outputs_keys = new List<string>(outputs.Keys);

			if (this is IRegisterDefaultPorts) {
				((IRegisterDefaultPorts)this).OnRegisterDefaultPorts();
			}
			if (this is IRegisterPorts) {
				((IRegisterPorts)this).OnRegisterPorts();
			}

			foreach (string key in old_port_keys) {
				if (!recently_modified.Contains(key)) {
					ports.Remove(key);
				}
			}
			foreach (string key in old_inputs_keys) {
				if (!recently_modified.Contains(key)) {
					inputs.Remove(key);
				}
			}
			foreach (string key in old_outputs_keys) {
				if (!recently_modified.Contains(key)) {
					outputs.Remove(key);
				}
			}

			var ports_sorted = ports.OrderBy(key => recently_modified.IndexOf(key.Key));
			ports = ports_sorted.ToDictionary((key_item) => key_item.Key, (value_item) => value_item.Value);

			var inputs_sorted = inputs.OrderBy(key => recently_modified.IndexOf(key.Key));
			inputs = inputs_sorted.ToDictionary((key_item) => key_item.Key, (value_item) => value_item.Value);

			var outputs_sorted = outputs.OrderBy(key => recently_modified.IndexOf(key.Key));
			outputs = outputs_sorted.ToDictionary((key_item) => key_item.Key, (value_item) => value_item.Value);

			port_keys = ports.Keys.ToList();
			input_keys = inputs.Keys.ToList();
			output_keys = outputs.Keys.ToList();

			port_values = ports.Values.ToList();
			input_values = inputs.Values.ToList();
			output_values = outputs.Values.ToList();
		}

		public void RegisterEvents() {
			if (this is IRegisterEvents) {
				((IRegisterEvents)this).OnRegisterEvents();
			}
		}

		public void RemoveEvents() {
			if (this is IRegisterEvents) {
				((IRegisterEvents)this).OnRemoveEvents();
			}
		}

		public Port GetPort(int id) {
			return portValues.FirstOrDefault(p => p.GetInstanceID() == id);
		}

		public Port GetPort(string name) {
			Port p;
			if (ports.TryGetValue(name, out p)) {
				return p;
			}
			else {
				return null;
			}
		}

		public void RemovePort(string name) {
#if UNITY_EDITOR
			is_ready = false;
#endif
			Port p;
			if (ports.TryGetValue(name, out p)) {
				if (p is IPlug) {
					((IPlug)p).Unplug();
				}
				ports.Remove(name);
			}
			Port i;
			if (inputs.TryGetValue(name, out i)) {
				if (i is IPlug) {
					((IPlug)i).Unplug();
				}
				inputs.Remove(name);
			}
			Port o;
			if (outputs.TryGetValue(name, out o)) {
				if (o is IPlug) {
					((IPlug)o).Unplug();
				}
				outputs.Remove(name);
			}
		}

		public void ClearPorts() {
			if (!recently_modified.IsNullOrEmpty()) {
				recently_modified.Clear();
			}
			foreach (Port port in portValues) {
				if (port is IPlug) {
					((IPlug)port).Unplug();
				}
				ports.Remove(port.name);
			}
			foreach (Port port in inputValues) {
				if (port is IPlug) {
					((IPlug)port).Unplug();
				}
				inputs.Remove(port.name);
			}
			foreach (Port port in outputValues) {
				if (port is IPlug) {
					((IPlug)port).Unplug();
				}
				outputs.Remove(port.name);
			}
		}

		public void ClearConnections() {
			foreach (Port port in portValues) {
				if (port is IPlug) {
					((IPlug)port).Unplug();
				}
			}
		}

		private T RegisterPort<T>(string name, T port, bool display_port = true) where T : Port {
			Port p;
			if (ports.TryGetValue(name, out p)) {
				p.display_port = display_port;
				return (T)p;
			}
			else {
				port.display_port = display_port;
				ports.Add(name, port);
				port.name = name;
				return port;
			}
		}

		int pass = 0;
		private T RegisterInputPort<T>(string name, T port, bool display_port = true) where T : Port {
			Port p;
			if (outputs.TryGetValue(name, out p)) {
				return RegisterInputPort<T>(string.Format("{0} ({1})", name, pass++), port, display_port);
			}
			else {
				pass = 0;
				return (T)(inputs[name] = RegisterPort(name, port, display_port));
			}
		}

		private T RegisterOutputPort<T>(string name, T port, bool display_port = true) where T : Port {
			Port p;
			if (inputs.TryGetValue(name, out p)) {
				return RegisterOutputPort<T>(string.Format("{0} ({1})", name, pass++), port, display_port);
			}
			else {
				pass = 0;
				return (T)(outputs[name] = RegisterPort(name, port, display_port));
			}
		}

		protected InputAction RegisterEntryPort(string name, Act action, bool display_port = true) {
			recently_modified.Add(name);
			Port p;
			if (inputs.TryGetValue(name, out p)) {
				p.display_port = display_port;
				InputAction input = (InputAction)p;
				if (input) {
					input.SetAction(action);
					return input;
				}
			}
			return RegisterInputPort(name, new InputAction(this, action), display_port);
		}

		protected OutputAction RegisterExitPort(string name, bool display_port = true) {
			recently_modified.Add(name);
			Port p;
			if (outputs.TryGetValue(name, out p)) {
				p.display_port = display_port;
				return (OutputAction)p;
			}
			else {
				return RegisterOutputPort(name, new OutputAction(this), display_port);
			}
		}

		protected InputValue<T> RegisterInputValue<T>(string name) {
			recently_modified.Add(name);
			Port p;
			if (inputs.TryGetValue(name, out p)) {
				p.display_port = true;
				return (InputValue<T>)p;
			}
			else {
				return RegisterInputPort(name, new InputValue<T>(this, default(T)), true);
			}
		}

		protected InputValue<T> RegisterInputValue<T>(string name, T default_value, bool display_port = true) {
			recently_modified.Add(name);
			Port p;
			if (inputs.TryGetValue(name, out p)) {
				p.display_port = display_port;
				return (InputValue<T>)p;
			}
			else {
				return RegisterInputPort(name, new InputValue<T>(this, default_value), display_port);
			}
		}

		protected OutputValue<T> RegisterOutputValue<T>(string name, ActValue<T> action, bool display_port = true) {
			recently_modified.Add(name);
			Port p;
			if (outputs.TryGetValue(name, out p)) {
				p.display_port = display_port;
				OutputValue<T> output = p as OutputValue<T>;
				if (output) {
					output.SetAction(action);
					return output;
				}
			}
			return RegisterOutputPort(name, new OutputValue<T>(this, action), display_port);
		}

		protected Port RegisterInputValue(Type type, string name, object default_value = default(object), bool display_port = true) {
			recently_modified.Add(name);
			Port p;
			if (inputs.TryGetValue(name, out p)) {
				p.display_port = display_port;
				return p;
			}
			else {
				return RegisterInputPort(name, (ValuePort)CreateGenericInstance(typeof(InputValue<>), type, this, default_value), display_port);
			}
		}

		protected Port RegisterOutputValue(Type type, string name, ActValue<object> action, bool display_port = true) {
			recently_modified.Add(name);
			Port p;
			if (outputs.TryGetValue(name, out p)) {
				p.display_port = display_port;
				IOutputValue output = p as IOutputValue;
				if (output != null) {
					output.SetAction(action);
					return (Port)output;
				}
			}
			return RegisterOutputPort(name, (ValuePort)CreateGenericInstance(typeof(OutputValue<>), type, this, type, action), display_port);
		}

		public List<Node> GetAllNodes() {
			List<Node> all_nodes = new List<Node>(nodes);
			foreach (Node node in nodes) {
				all_nodes = all_nodes.Concat(node.GetAllNodes()).ToList();
			}
			return all_nodes;
		}

		public List<T> GetNodesOfType<T>() where T : Node {
			return nodes.OfType<T>().ToList();
		}

		public List<Graph> GetGraphs() {
			return nodes.OfType<Graph>().ToList();
		}

		public List<Graph> GetAllGraphs() {
			List<Graph> all_graphs = new List<Graph>(GetNodesOfType<Graph>());
			foreach (Graph graph in GetNodesOfType<Graph>()) {
				all_graphs = all_graphs.Concat(graph.GetAllGraphs()).ToList();
			}
			return all_graphs;
		}

		public Node GetClone() {
			Node instance = SerializationUtility.DeserializeValue<Node>(SerializationUtility.SerializeValue(this, DataFormat.Binary), DataFormat.Binary);
			instance.ChangeID();
			instance.nodes = new List<Node>();
			//instance.ClearPorts();
			Dictionary<string, Port> copy = new Dictionary<string, Port>();
			foreach (Port port in instance.portValues) {
				Port port_instance = SerializationUtility.DeserializeValue<Port>(SerializationUtility.SerializeValue(port, DataFormat.Binary), DataFormat.Binary);
				port_instance.ChangeID();
				string key = port.name;
				instance.ports.Remove(key);
				instance.ports[key] = port_instance;
				copy[key] = port_instance;
				((IPlug)port_instance).Unplug();
			}
			foreach (Port port in instance.inputValues) {
				string key = port.name;
				Port p;
				if (copy.TryGetValue(key, out p)) {
					instance.inputs.Remove(key);
					instance.inputs[key] = p;
				}
				else {
					instance.inputs.Remove(key);
				}
			}
			foreach (Port port in instance.outputValues) {
				string key = port.name;
				Port p;
				if (copy.TryGetValue(key, out p)) {
					instance.outputs.Remove(key);
					instance.outputs[key] = p;
				}
				else {
					instance.outputs.Remove(key);
				}
			}
			foreach (Node node in nodes) {
				instance.nodes.Add(node.GetClone());
			}
			instance.Validate();
			return instance;
		}
	}
}
