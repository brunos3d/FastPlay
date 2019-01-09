using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Equals")]
	[Subtitle("Integer")]
	[Path("Functions/Operation/Integer/Equals")]
	public class IntEquals : ValueNode<bool>, IRegisterPorts {

		public InputValue<int> a, b;

		protected override string getDefaultName { get { return "A == B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<int>("A");
			b = RegisterInputValue<int>("B");
		}

		public override bool OnGetValue() {
			return a.value == b.value;
		}
	}
}
