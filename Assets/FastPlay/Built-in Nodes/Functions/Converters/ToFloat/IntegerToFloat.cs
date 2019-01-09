using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("ToFloat")] 
	[Path("Functions/Converters/ToFloat/Integer")]
	public class IntegerToFloat : ValueNode<float>, IRegisterPorts {

		public InputValue<int> i;

		public void OnRegisterPorts() {
			i = RegisterInputValue<int>("int");
		}

		public override float OnGetValue() {
			return i.value;
		}
	}
}
