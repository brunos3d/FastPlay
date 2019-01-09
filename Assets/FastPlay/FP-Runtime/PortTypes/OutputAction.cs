using System;
using UnityEngine;

namespace FastPlay.Runtime {
	public class OutputAction : ActionPort, IOutputPort, IPlugIn {

		public InputAction input;

		public OutputAction() { }

		public OutputAction(Node node) {
			this.node = node;
		}

		public override void OnCall() {
			try {
				if (input) {
					input.Call();
				}
			}
			catch (Exception exception) {
				Debug.LogException(exception);
			}
		}

		public bool IsPlugged() {
			return input != null;
		}

		public IPlugOut GetPluggedPort() {
			return input;
		}

		public bool CanPlug(IPlugOut plug, bool overwrite = true) {
			Port port = plug as Port;
			return (overwrite ? true : !IsPlugged()) && port && port.node != node && port is InputAction;
		}

		public void PlugTo(IPlugOut port) {
			if (CanPlug(port)) {
				if (IsPlugged()) {
					GetPluggedPort().UnplugFrom(this);
				}
				input = (InputAction)port;
				port.AddPlug(this);
			}
		}

		public void Unplug() {
			if (IsPlugged()) {
				GetPluggedPort().UnplugFrom(this);
				input = null;
			}
		}
	}
}
