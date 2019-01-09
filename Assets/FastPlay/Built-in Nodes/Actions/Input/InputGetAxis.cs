using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Input.GetAxis")]
	[Path("Actions/Input/Input.GetAxis")]
	[Body("GetAxis", "Input")]
	public class InputGetAxis : ValueNode<float>, IRegisterPorts {

		public InputValue<string> axis_name;

		public void OnRegisterPorts() {
			axis_name = RegisterInputValue<string>("axisName");
		}

		public override float OnGetValue() {
			return Input.GetAxis(axis_name.value);
		}
	}
}
