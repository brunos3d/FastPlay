using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FastPlay.Runtime {
	[HideInList]
	public partial class Graph : Node {

		[NonSerialized]
		public bool is_enabled;

		[NonSerialized]
		public List<IUpdate> updatable_nodes = new List<IUpdate>();

		[SerializeField]
		private List<Parameter> var_parameters = new List<Parameter>();

		[SerializeField]
		private List<Parameter> in_parameters = new List<Parameter>();

		[SerializeField]
		private List<Parameter> out_parameters = new List<Parameter>();

		public List<Parameter> variableParameters {
			get {
				return var_parameters ?? (var_parameters = new List<Parameter>());
			}
			set {
				var_parameters = value;
			}
		}

		public List<Parameter> inputParameters {
			get {
				return in_parameters ?? (in_parameters = new List<Parameter>());
			}
			set {
				in_parameters = value;
			}
		}

		public List<Parameter> outputParameters {
			get {
				return out_parameters ?? (out_parameters = new List<Parameter>());
			}
			set {
				out_parameters = value;
			}
		}

		public override void Validate() {
			graph = graph ?? this;
			RemoveNullFromNodes();

			foreach (Node node in nodes) {
				node.graph = node.graph ?? this;
				node.Validate();
			}
#if UNITY_EDITOR
			EDITOR_Prepare();
#endif
		}

		public override void Initialize() {
			base.Initialize();
			foreach (Node node in nodes) {
				node.Initialize();
			}
		}

		public override void Finish() {
			RemoveEvents();
			foreach (Port port in portValues) {
				port.Finish();
			}
			foreach (Node node in nodes) {
				node.Finish();
			}
		}

		public void Play() {
			Initialize();

			updatable_nodes = nodes.OfType<IUpdate>().ToList();
			foreach (Node node in nodes) {
				node.OnGraphPlay();
			}

			if (!is_enabled) {
				// After graph prepared call custom event (OnEnable)
				foreach (OnEnableEvent node in nodes.OfType<OnEnableEvent>()) {
					node.OnEnable();
				}
				is_enabled = true;
			}

			foreach (StartEvent node in nodes.OfType<StartEvent>()) {
				node.Start();
			}
		}

		public void Stop() {
			if (is_enabled) {
				foreach (OnDisableEvent node in nodes.OfType<OnDisableEvent>()) {
					node.OnDisable();
				}
				is_enabled = false;
			}

			foreach (Node node in nodes) {
				node.OnGraphStop();
			}
		}

		public void Update() {
			foreach (IUpdate node in updatable_nodes) {
				Call(node.Update);
			}
		}

		public Parameter<T> GetParameter<T>(string name, ParameterType param_type) {
			switch (param_type) {
				case ParameterType.None:
					return (Parameter<T>)variableParameters.Concat(inputParameters).Concat(outputParameters).FirstOrDefault(p => p.name == name);
				case ParameterType.Variable:
					return (Parameter<T>)variableParameters.First(p => p.name == name);
				case ParameterType.Input:
					return (Parameter<T>)inputParameters.First(p => p.name == name);
				case ParameterType.Output:
					return (Parameter<T>)outputParameters.First(p => p.name == name);
				default:
					goto case ParameterType.None;
			}
		}

		public void RemoveParameter(string name, ParameterType param_type) {
			switch (param_type) {
				case ParameterType.None:
					variableParameters.Remove(variableParameters.Find(p => p.name == name));
					inputParameters.Remove(inputParameters.Find(p => p.name == name));
					outputParameters.Remove(outputParameters.Find(p => p.name == name));
					return;
				case ParameterType.Variable:
					variableParameters.Remove(variableParameters.Find(p => p.name == name));
					return;
				case ParameterType.Input:
					inputParameters.Remove(inputParameters.Find(p => p.name == name));
					return;
				case ParameterType.Output:
					outputParameters.Remove(outputParameters.Find(p => p.name == name));
					return;
				default:
					goto case ParameterType.None;
			}
		}

		public void RemoveParameter(int key, ParameterType param_type) {
			switch (param_type) {
				case ParameterType.None:
					variableParameters.Remove(variableParameters.Find(p => p.id == key));
					inputParameters.Remove(inputParameters.Find(p => p.id == key));
					outputParameters.Remove(outputParameters.Find(p => p.id == key));
					return;
				case ParameterType.Input:
					inputParameters.Remove(inputParameters.Find(p => p.id == key));
					return;
				case ParameterType.Variable:
					variableParameters.Remove(variableParameters.Find(p => p.id == key));
					return;
				case ParameterType.Output:
					outputParameters.Remove(outputParameters.Find(p => p.id == key));
					return;
				default:
					goto case ParameterType.None;
			}
		}

		public void RemoveParameterAt(int index, ParameterType param_type) {
			switch (param_type) {
				case ParameterType.None:
					variableParameters.RemoveAt(index);
					inputParameters.RemoveAt(index);
					outputParameters.RemoveAt(index);
					return;
				case ParameterType.Variable:
					variableParameters.RemoveAt(index);
					return;
				case ParameterType.Input:
					inputParameters.RemoveAt(index);
					return;
				case ParameterType.Output:
					outputParameters.RemoveAt(index);
					return;
				default:
					goto case ParameterType.None;
			}
		}

		public Parameter AddCustomParameter(string name, Type type, ParameterType param_type) {
			Parameter instance = (Parameter)ObjectBase.CreateInstance(type, name.IsNullOrEmpty() ? "new " + type.GetTypeName() : name);
			switch (param_type) {
				case ParameterType.None:
					variableParameters.Add(instance);
					inputParameters.Add(instance);
					outputParameters.Add(instance);
					return instance;
				case ParameterType.Variable:
					variableParameters.Add(instance);
					return instance;
				case ParameterType.Input:
					inputParameters.Add(instance);
					return instance;
				case ParameterType.Output:
					outputParameters.Add(instance);
					return instance;
				default:
					goto case ParameterType.None;
			}
		}

		public Parameter AddParameter(string name, Type type, ParameterType param_type) {
			Parameter instance = (Parameter)ObjectBase.CreateGenericInstance(typeof(Parameter<>), type, name.IsNullOrEmpty() ? "new " + type.GetTypeName() : name);
			switch (param_type) {
				case ParameterType.None:
					variableParameters.Add(instance);
					inputParameters.Add(instance);
					outputParameters.Add(instance);
					return instance;
				case ParameterType.Variable:
					variableParameters.Add(instance);
					return instance;
				case ParameterType.Input:
					inputParameters.Add(instance);
					return instance;
				case ParameterType.Output:
					outputParameters.Add(instance);
					return instance;
				default:
					goto case ParameterType.None;
			}
		}

		public Parameter<T> AddParameter<T>(string name, ParameterType param_type) {
			Parameter<T> instance = CreateInstance<Parameter<T>>(name);
			switch (param_type) {
				case ParameterType.None:
					variableParameters.Add(instance);
					inputParameters.Add(instance);
					outputParameters.Add(instance);
					return instance;
				case ParameterType.Variable:
					variableParameters.Add(instance);
					return instance;
				case ParameterType.Input:
					inputParameters.Add(instance);
					return instance;
				case ParameterType.Output:
					outputParameters.Add(instance);
					return instance;
				default:
					goto case ParameterType.None;
			}
		}

		public void RemoveNullFromNodes() {
			if (nodes == null) {
				nodes = new List<Node>();
			}
			nodes.RemoveAll(n => n == null);
		}

		public MacroNode AddMacro(GraphAsset reference, Vector2 position = default(Vector2)) {
			if (!reference) return null;
			MacroNode instance = AddNode<MacroNode>(position, false);
			instance.reference = reference;
			instance.Validate();
			instance.OnGraphAdd();
			return instance;
		}

		public T AddNode<T>(Vector2 position = default(Vector2), bool validate = true) where T : Node {
			return (T)AddNode(typeof(T), position, validate);
		}

		public T AddCustomNode<T>(Vector2 position = default(Vector2), bool validate = true, params object[] args) where T : Node {
			return (T)AddCustomNode(typeof(T), position, validate, args);
		}

		public Node AddCustomNode(Type type, Vector2 position = default(Vector2), bool validate = true, params object[] args) {
			if (!typeof(Node).IsAssignableFrom(type)) return null;
			Node instance = (Node)CreateInstance(type, args);
			nodes.Add(instance);
			instance.graph = this;
#if UNITY_EDITOR
			instance.position = position;
#endif
			if (validate) {
				instance.Validate();
				instance.OnGraphAdd();
			}
			return instance;
		}

		public Node AddNode(Type type, Vector2 position = default(Vector2), bool validate = true) {
			if (!typeof(Node).IsAssignableFrom(type)) return null;
			Node instance = (Node)CreateInstance(type);
			nodes.Add(instance);
			instance.graph = this;
#if UNITY_EDITOR
			instance.position = position;
#endif
			if (validate) {
				instance.Validate();
				instance.OnGraphAdd();
			}
			return instance;
		}

		public Node AddNode(Node node, Vector2 position = default(Vector2)) {
			nodes.Add(node);
			node.graph = this;
#if UNITY_EDITOR
			node.position = position;
#endif
			node.OnGraphAdd();
			return node;
		}

		public void RemoveNodes(List<Node> nodes) {
			if (nodes.Count > 0) {
				foreach (Node node in nodes) {
					RemoveNode(node);
				}
			}
		}

		public void RemoveNode(Node node) {
			foreach (Port port in node.portValues) {
				if (port is IPlug) {
					((IPlug)port).Unplug();
				}
			}
			nodes.Remove(node);
		}
	}
}
