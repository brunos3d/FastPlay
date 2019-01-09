using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Sequence")]
	[Subtitle("Flow Control")]
	[Path("Flow Control/Sequence")]
	public class Sequence : ActionNode, IRegisterPorts, IListPort {

		public List<OutputAction> m_outputs = new List<OutputAction>();

		public int port_count = 5;

		public int portCount {
			get {
				return port_count < 0 ? port_count = 0 : port_count;
			}
			set {
				port_count = value < 0 ? 0 : value;
			}
		}

		public void OnRegisterPorts() {
			for (int id = 0; id < portCount; id++) {
				RegisterPort(string.Format("Then {0}", id));
			}
		}

		public void AddPort() {
			portCount++;
		}

		public void RemovePort() {
			string port_name = string.Format("Then {0}", m_outputs.Count - 1);
			m_outputs.Remove((OutputAction)GetPort(port_name));
			base.RemovePort(port_name);
			portCount--;
		}

		public void RegisterPort(string name) {
			OutputAction output = RegisterExitPort(name);
			if (m_outputs.Contains(output)) return;
			m_outputs.Add(output);
		}

		public override void OnExecute() {
			foreach (OutputAction output in m_outputs) {
				Call(output);
			}
			Call(output);
		}
	}
}
