using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Divide")]
	[Path("Functions/Operation/Integer/Divide")]
	[Body("Divide", "Integer")]
	public class IntDivide : ValueNode<int>, IRegisterPorts {

		public InputValue<int> a, b;

		protected override string getDefaultName { get { return "A / B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<int>("A");
			b = RegisterInputValue<int>("B");
		}

		public override int OnGetValue() {
			return a.value / b.value;
		}
	}
}
