using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Negative")]
	[Subtitle("Boolean")]
	[Path("Functions/Operation/Boolean/Negative")]
	public class BoolNegative : ValueNode<bool>, IRegisterPorts {

		public InputValue<bool> value;

		protected override string getDefaultName { get { return "!value"; } }

		public void OnRegisterPorts() {
			value = RegisterInputValue<bool>("value");
		}

		public override bool OnGetValue() {
			return !value.value;
		}
	}
}
