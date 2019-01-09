using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Subtract")]
	[Path("Functions/Operation/Integer/Subtract")]
	[Body("Subtract", "Integer")]
	public class IntSubtract : ValueNode<int>, IRegisterPorts {

		public InputValue<int> a, b;

		protected override string getDefaultName { get { return "A - B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<int>("A");
			b = RegisterInputValue<int>("B");
		}

		public override int OnGetValue() {
			return a.value - b.value;
		}
	}
}
