using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("ToInteger")]
	[Path("Functions/Converters/ToInteger/String")]
	public class StringToInteger : ValueNode<int>, IRegisterPorts {

		public InputValue<string> s;

		public void OnRegisterPorts() {
			s = RegisterInputValue<string>("string");
		}

		public override int OnGetValue() {
			return int.Parse(s.value);
		}
	}
}
