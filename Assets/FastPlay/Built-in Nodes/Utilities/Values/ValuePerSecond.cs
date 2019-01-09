using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("PerSecond")]
	[Path("Utilities/Values/PerSecond")]
	[Body("PerSecond","", "Time Icon")]
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
