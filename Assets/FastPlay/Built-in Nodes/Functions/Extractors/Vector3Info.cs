using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Vector3 Info")]
	[Path("Functions/Extractors/Vector3")]
	[Body("Info", "Vector3")]
	public class Vector3Info : MultiValueNode<float, float, float>, IRegisterPorts {

		public InputValue<Vector3> vector;

		public override string[] portNames {
			get {
				return new string[] { "X", "Y", "Z" };
			}
		}

		public void OnRegisterPorts() {
			vector = RegisterInputValue<Vector3>("vector3");
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
	}
}
