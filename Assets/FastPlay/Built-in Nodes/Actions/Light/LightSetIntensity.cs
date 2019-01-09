using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("SetIntensity")]
	[Path("Actions/Light/SetIntensity")]
	[Body("SetIntensity", "Light")]
	public class LightSetIntensity : ActionNode, IRegisterPorts {

		public InputValue<Light> target;
		public InputValue<float> intensity;

		public void OnRegisterPorts() {
			target = RegisterInputValue<Light>("target");
			intensity = RegisterInputValue<float>("intensity");
		}

		public override void OnExecute() {
			target.value.intensity = intensity.value;
			Call(output);
		}
	}
}
