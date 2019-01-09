using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("ToFloat")]
	[Path("Functions/Converters/ToFloat/String")]
	[Body("ToFloat", "Single")]
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
