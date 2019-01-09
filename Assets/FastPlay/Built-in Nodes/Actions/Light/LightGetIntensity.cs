using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("GetIntensity")]
	[Path("Actions/Light/GetIntensity")]
	[Body("GetIntensity", "Light")]
	public class LightGetIntensity : ValueNode<float>, IRegisterPorts {

		public InputValue<Light> target;

		public void OnRegisterPorts() {
			target = RegisterInputValue<Light>("target");
		}

		public override float OnGetValue() {
			return target.value.intensity;
		}
	}
}
