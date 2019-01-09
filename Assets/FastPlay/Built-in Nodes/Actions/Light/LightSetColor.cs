using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("SetColor")]
	[Path("Actions/Light/SetColor")]
	[Body("SetColor", "Light")]
	public class LightSetColor : ActionNode, IRegisterPorts {

		public InputValue<Light> target;
		public InputValue<Color> color;

		public void OnRegisterPorts() {
			target = RegisterInputValue<Light>("target");
			color = RegisterInputValue<Color>("color");
		}

		public override void OnExecute() {
			target.value.color = color.value;
			Call(output);
		}
	}
}
