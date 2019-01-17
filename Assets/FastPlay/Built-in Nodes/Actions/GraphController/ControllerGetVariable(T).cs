using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("GetVariable")]
	[Subtitle("GraphController")]
	[Path("Actions/GraphController/GetVariable<T>")]
	public class ControllerGetVariable<T> : ValueNode<VariableObject<T>>, IRegisterPorts {

		private VariableObject<T> variable_cache;

		public InputValue<GraphController> controller;

		public InputValue<int> variable_key;

		public void OnRegisterPorts() {
			controller = RegisterInputValue<GraphController>("controller");
			variable_key = RegisterInputValue<int>("key");
		}

		public override VariableObject<T> OnGetValue() {
			return (variable_cache ?? (variable_cache = controller.value.GetVariable<T>(variable_key.value)));
		}
	}
}
