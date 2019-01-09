using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Subtract")]
	[Path("Functions/Operation/Vector3/Subtract")]
	[Body("Subtract", "Vector3")]
	public class Vector3Subtract : ValueNode<Vector3>, IRegisterPorts {

		public InputValue<Vector3> a, b;

		protected override string getDefaultName { get { return "A - B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<Vector3>("A");
			b = RegisterInputValue<Vector3>("B");
		}

		public override Vector3 OnGetValue() {
			return a.value - b.value;
		}
	}
}
