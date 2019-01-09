using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Equals")]
	[Subtitle("String")]
	[Path("Functions/Operation/String/Equals")]
	public class StringEquals : ValueNode<bool>, IRegisterPorts {

		public InputValue<string> a, b;

		protected override string getDefaultName { get { return "A == B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<string>("A");
			b = RegisterInputValue<string>("B");
		}

		public override bool OnGetValue() {
			return a.value == b.value;
		}
	}
}
