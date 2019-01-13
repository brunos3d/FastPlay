namespace FastPlay.Runtime {
	public class InputValue<T> : ValuePort<T>, IInputValue, IPlugIn {

		public IOutputValue plugged_port;

		public T default_value;

		public GetFilter get_filter = GetFilter.DefaultValue;

		public T value {
			get {
#if UNITY_EDITOR
				flow_state = FlowState.Active;
#endif
				switch (get_filter) {
					case GetFilter.Action:
						return (T)plugged_port.GetValue();
					case GetFilter.DefaultValue:
						return default_value;
					default:
						return default_value;
				}
			}
			set {
				default_value = value;
			}
		}

		public InputValue() { }

		public InputValue(Node node, T default_value = default(T)) {
			this.node = node;
			this.default_value = default_value;
			get_filter = GetFilter.DefaultValue;
		}

		public override void Initialize() {
			if (get_filter == GetFilter.DefaultValue) {
				if (typeof(UnityEngine.Component).IsAssignableFrom(typeof(T))) {
					default_value = (T)(object)Current.GetComponent(valueType);
				}
				else if (typeof(UnityEngine.GameObject).IsAssignableFrom(typeof(T))) {
					default_value = (T)(object)Current.gameObject;
				}
				else if (typeof(Graph).IsAssignableFrom(typeof(T))) {
					default_value = (T)(object)Current.graph;
				}
			}
		}

		public object GetValue() {
			return value;
		}

		public object GetDefaultValue() {
			return default_value;
		}

		public void SetDefaultValue(object value) {
			default_value = (T)value;
		}

		public bool IsPlugged() {
			return plugged_port != null;
		}

		public IPlugOut GetPluggedPort() {
			return (IPlugOut)plugged_port;
		}

		public bool CanPlug(IPlugOut plug, bool overwrite = true) {
			Port port = plug as Port;
			return this.display_port && ((overwrite & (port && port.display_port) ? true : !IsPlugged()) && port && port.display_port && port.node != node && port is IOutputValue && typeof(T).IsAssignableFrom(((IOutputValue)port).valueType));
		}

		public void PlugTo(IPlugOut port) {
			if (CanPlug(port)) {
				if (IsPlugged()) {
					GetPluggedPort().UnplugFrom(this);
				}
				plugged_port = (IOutputValue)port;
				get_filter = GetFilter.Action;
				port.AddPlug(this);
			}
		}

		public void Unplug() {
			if (IsPlugged()) {
				GetPluggedPort().UnplugFrom(this);
			}
			plugged_port = null;
			get_filter = GetFilter.DefaultValue;
		}


		public static implicit operator T(InputValue<T> port) {
			return port.value;
		}
	}
}
