using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("Time Icon")]
	[Title("PerSecond")]
	[Path("Utilities/Values/PerSecond")]
	public class ValuePerSecond : ValueNode<float>, IRegisterPorts {

		public InputValue<float> value;

		public void OnRegisterPorts() {
			value = RegisterInputValue<float>("value");
		}

		public override float OnGetValue() {
			return Time.deltaTime * value.value;
		}
	}
}
