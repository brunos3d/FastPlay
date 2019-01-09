using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Input.GetAxisRaw")]
	[Path("Actions/Input/Input.GetAxisRaw")]
	[Body("GetAxisRaw", "Input")]
	public class InputGetAxisRaw : ValueNode<float>, IRegisterPorts {

		public InputValue<string> axis_name;

		public void OnRegisterPorts() {
			axis_name = RegisterInputValue<string>("axisName");
		}

		public override float OnGetValue() {
			return Input.GetAxisRaw(axis_name.value);
		}
	}
}
