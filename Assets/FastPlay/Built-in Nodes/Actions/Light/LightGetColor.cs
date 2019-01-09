using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("GetColor")]
	[Path("Actions/Light/GetColor")]
	[Body("GetColor", "Light")]
	public class LightGetColor : ValueNode<Color>, IRegisterPorts {

		public InputValue<Light> target;

		public void OnRegisterPorts() {
			target = RegisterInputValue<Light>("target");
		}

		public override Color OnGetValue() {
			return target.value.color;
		}
	}
}
