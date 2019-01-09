using System.Collections;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("ForEach")]
	[Path("Flow Control/ForEach")]
	[Body("ForEach", "Flow Control")]
	public class ForEach : ActionNode, IRegisterPorts {

		public object current;
		public bool broken;
		public OutputAction on_loop;
		public InputValue<IEnumerable> list;

		public void OnRegisterPorts() {
			list = RegisterInputValue<IEnumerable>("list");

			RegisterEntryPort("Break", () => { broken = true; });
			RegisterOutputValue<object>("current", () => { return current; });
			on_loop = RegisterExitPort("Loop");
		}

		public override void OnExecute() {
			broken = false;
			if (list.value == null) {
				Call(output);
				return;
			}
			foreach (object element in list.value) {
				if (broken) {
					break;
				}
				current = element;
				Call(on_loop);
			}
			Call(output);
		}
	}
}
