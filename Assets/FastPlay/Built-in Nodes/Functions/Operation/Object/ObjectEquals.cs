using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Equals")]
	[Subtitle("Object")]
	[Path("Functions/Operation/Object/Equals")]
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
