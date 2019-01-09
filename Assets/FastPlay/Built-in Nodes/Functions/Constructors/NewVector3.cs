using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New Vector3")]
	[Path("Functions/Constructors/New Vector3")]
	[Body("New Vector3")]
	public class NewVector3 : ValueNode<Vector3>, IRegisterPorts {

		public InputValue<float> x, y, z;

		public void OnRegisterPorts() {
			Vector3 defaultVector3 = default(Vector3);
			x = RegisterInputValue<float>("X", defaultVector3.x);
			y = RegisterInputValue<float>("Y", defaultVector3.y);
			z = RegisterInputValue<float>("Z", defaultVector3.z);
		}

		public override Vector3 OnGetValue() {
			return new Vector3(x.value, y.value, z.value);
		}
	}
}
