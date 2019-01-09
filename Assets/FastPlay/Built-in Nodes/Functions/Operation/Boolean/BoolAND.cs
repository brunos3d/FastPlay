using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("AND")]
	[Subtitle("Boolean")]
	[Path("Functions/Operation/Boolean/AND")]
	public class BoolAND : ValueNode<bool>, IRegisterPorts {

		public InputValue<bool> a, b;

		protected override string getDefaultName { get { return "A && B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<bool>("A");
			b = RegisterInputValue<bool>("B");
		}

		public override bool OnGetValue() {
			return a.value && b.value;
		}
	}
}
