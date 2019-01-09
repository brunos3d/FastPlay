using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New Vector4")]
	[Path("Functions/Constructors/New Vector4")]
	[Body("New Vector4")]
	public class NewVector4 : ValueNode<Vector4>, IRegisterPorts {

		public InputValue<float> x, y, z, w;

		public void OnRegisterPorts() {
			Vector4 defaultVector4 = default(Vector4);
			x = RegisterInputValue<float>("X", defaultVector4.x);
			y = RegisterInputValue<float>("Y", defaultVector4.y);
			z = RegisterInputValue<float>("Z", defaultVector4.z);
			w = RegisterInputValue<float>("W", defaultVector4.w);
		}

		public override Vector4 OnGetValue() {
			return new Vector4(x.value, y.value, z.value, w.value);
		}
	}
}
