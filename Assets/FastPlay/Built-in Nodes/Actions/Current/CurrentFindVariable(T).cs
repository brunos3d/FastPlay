using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("FindVariable")]
	[Subtitle("Current")]
	[Path("Actions/Current/FindVariable<T>")]
	public class CurrentFindVariable<T> : ValueNode<VariableObject<T>>, IRegisterPorts {

		public InputValue<string> variable_name;

		public CurrentFindVariable() { }

		public void OnRegisterPorts() {
			variable_name = RegisterInputValue<string>("name");
		}

		public override VariableObject<T> OnGetValue() {
			return Current.FindVariable<T>(variable_name.value);
		}
	}
}
