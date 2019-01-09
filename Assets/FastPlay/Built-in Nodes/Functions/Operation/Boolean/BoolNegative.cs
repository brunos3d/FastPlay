using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Negative")]
	[Path("Functions/Operation/Boolean/Negative")]
	[Body("Negative", "Boolean")]
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
