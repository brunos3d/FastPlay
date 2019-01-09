using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("ToInteger")]
	[Path("Functions/Converters/ToInteger/Float")]
	[Body("ToInteger", "Single")]
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
