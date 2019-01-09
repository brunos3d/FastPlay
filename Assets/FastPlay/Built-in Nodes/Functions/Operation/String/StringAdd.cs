using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Add")]
	[Subtitle("String")]
	[Path("Functions/Operation/String/Add")]
	public class StringAdd : ValueNode<string>, IRegisterPorts {

		public InputValue<string> a, b;

		protected override string getDefaultName { get { return "A + B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<string>("A");
			b = RegisterInputValue<string>("B");
		}

		public override string OnGetValue() {
			return a.value + b.value;
		}
	}
}
