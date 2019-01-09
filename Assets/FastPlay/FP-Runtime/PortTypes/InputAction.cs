using System.Collections.Generic;

namespace FastPlay.Runtime {
	public class InputAction : ActionPort, IInputPort, IPlugOut {

		public event Act action;

		public List<IPlugIn> connections = new List<IPlugIn>();

		public InputAction() { }

		public InputAction(Node node, Act action) {
			this.node = node;
			this.action = action;
		}

		public override void OnCall() {
			action();
		}

		public void SetAction(Act action) {
			this.action = action;
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
	}
}
