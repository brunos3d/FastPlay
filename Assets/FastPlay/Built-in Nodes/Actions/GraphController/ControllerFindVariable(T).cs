using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("FindVariable")]
	[Subtitle("GraphController")]
	[Path("Actions/GraphController/FindVariable<T>")]
	public class ControllerFindVariable<T> : ValueNode<VariableObject<T>>, IRegisterPorts {

		private VariableObject<T> variable_cache;

		public InputValue<GraphController> controller;

		public InputValue<string> variable_name;

		public void OnRegisterPorts() {
			controller = RegisterInputValue<GraphController>("controller");
			variable_name = RegisterInputValue<string>("name");
		}

		public override VariableObject<T> OnGetValue() {
			return (variable_cache ?? (variable_cache = controller.value.FindVariable<T>(variable_name.value)));
		}
	}
}
