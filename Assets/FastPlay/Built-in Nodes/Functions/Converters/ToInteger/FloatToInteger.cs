using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("ToInteger")]
	[Path("Functions/Converters/ToInteger/Float")]
	public class FloatToInteger : ValueNode<int>, IRegisterPorts {

		public InputValue<float> f;

		public void OnRegisterPorts() {
			f = RegisterInputValue<float>("float");
		}

		public override int OnGetValue() {
			return (int)f.value;
		}
	}
}
