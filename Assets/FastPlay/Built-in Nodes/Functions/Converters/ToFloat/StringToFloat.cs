using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("ToFloat")]
	[Path("Functions/Converters/ToFloat/String")]
	public class StringToFloat : ValueNode<float>, IRegisterPorts {

		public InputValue<string> s;

		public void OnRegisterPorts() {
			s = RegisterInputValue<string>("string");
		}

		public override float OnGetValue() {
			return float.Parse(s.value);
		}
	}
}
