using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Vector4 Info")]
	[Path("Functions/Extractors/Vector4")]
	[Body("Info", "Vector4")]
	public class Vector4Info : MultiValueNode<float, float, float, float>, IRegisterPorts {

		public InputValue<Vector4> vector;

		public override string[] portNames {
			get {
				return new string[] { "X", "Y", "Z", "W" };
			}
		}

		public void OnRegisterPorts() {
			vector = RegisterInputValue<Vector4>("vector4");
		}

		public override float Get_0() {
			return vector.value.x;
		}

		public override float Get_1() {
			return vector.value.y;
		}

		public override float Get_2() {
			return vector.value.z;
		}

		public override float Get_3() {
			return vector.value.w;
		}
	}
}
