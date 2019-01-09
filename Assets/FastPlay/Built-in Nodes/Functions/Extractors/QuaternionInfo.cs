using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Quaternion Info")]
	[Path("Functions/Extractors/Quaternion")]
	[Body("Info", "Quaternion")]
	public class QuaternionInfo : MultiValueNode<float, float, float, float>, IRegisterPorts {

		public InputValue<Quaternion> quaternion;

		public override string[] portNames {
			get {
				return new string[] { "X", "Y", "Z", "W" };
			}
		}

		public void OnRegisterPorts() {
			quaternion = RegisterInputValue<Quaternion>("quaternion");
		}

		public override float Get_0() {
			return quaternion.value.x;
		}

		public override float Get_1() {
			return quaternion.value.y;
		}

		public override float Get_2() {
			return quaternion.value.z;
		}

		public override float Get_3() {
			return quaternion.value.w;
		}
	}
}
