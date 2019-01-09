using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Equals")]
	[Path("Functions/Operation/Object/Equals")]
	[Body("Equals", "Object")]
	public class ObjectEquals : ValueNode<bool>, IRegisterPorts {

		public InputValue<object> a, b;

		protected override string getDefaultName { get { return "A == B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<object>("A");
			b = RegisterInputValue<object>("B");
		}

		public override bool OnGetValue() {
			return a.value == b.value;
		}
	}
}
