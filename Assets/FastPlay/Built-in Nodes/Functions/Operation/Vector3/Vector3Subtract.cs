using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Subtract")]
	[Subtitle("Vector3")]
	[Path("Functions/Operation/Vector3/Subtract")]
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
