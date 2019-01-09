using System;
using System.Collections.Generic;

namespace FastPlay.Runtime {
	public class OutputValue<T> : ValuePort<T>, IOutputValue, IPlugOut {

		public event ActValue<T> action;

		public List<IPlugIn> connections = new List<IPlugIn>();

		public OutputValue() { }

		public OutputValue(Node node) {
			this.node = node;
		}

		public OutputValue(Node node, ActValue<T> action) {
			this.node = node;
			this.action = action;
		}

		// The cast parameter is used to differentiate the constructor
		public OutputValue(Node node, Type type, ActValue<object> action) {
			this.node = node;
			if (typeof(T).IsAssignableFrom(type)) {
				this.action = () => {
					return (T)action();
				};
			}
		}

		public object GetValue() {
#if UNITY_EDITOR
			flow_state = FlowState.Active;
#endif
			return action();
		}

		public void SetAction(ActValue<T> action) {
			this.action = action;
		}

		public void SetAction(ActValue<object> action) {
			this.action = () => {
#if UNITY_EDITOR
				flow_state = FlowState.Active;
#endif
				return (T)action();
			};
		}

		public bool IsPlugged() {
			return connections != null && connections.Count > 0;
		}

		public List<IPlugIn> GetPluggedPorts() {
			return connections;
		}

		public void AddPlug(IPlugIn port) {
			if (!connections.Contains(port)) {
				connections.Add(port);
			}
		}

		public void UnplugFrom(IPlugIn port) {
			connections.Remove(port);
		}

		public void Unplug() {
			while (connections.Count > 0) {
				connections[0].Unplug();
			}
			connections.Clear();
		}

		public static implicit operator T(OutputValue<T> port) {
			return port.action();
		}
	}
}
