using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New Quaternion")]
	[Path("Functions/Constructors/New Quaternion")]
	[Body("New Quaternion")]
	public class NewQuaternion : ValueNode<Quaternion>, IRegisterPorts {

		public InputValue<float> x, y, z, w;

		public void OnRegisterPorts() {
			Quaternion defaultQuaternion = default(Quaternion);
			x = RegisterInputValue<float>("X", defaultQuaternion.x);
			y = RegisterInputValue<float>("Y", defaultQuaternion.y);
			z = RegisterInputValue<float>("Z", defaultQuaternion.z);
			w = RegisterInputValue<float>("W", defaultQuaternion.w);
		}

		public override Quaternion OnGetValue() {
			return new Quaternion(x.value, y.value, z.value, w.value);
		}
	}
}
