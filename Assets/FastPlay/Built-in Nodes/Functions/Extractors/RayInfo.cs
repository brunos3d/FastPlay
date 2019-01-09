using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Ray Info")]
	[Path("Functions/Extractors/Ray")]
	[Body("Info", "Ray")]
	public class RayInfo : MultiValueNode<Vector3, Vector3>, IRegisterPorts {

		public InputValue<Ray> ray;

		public override string[] portNames {
			get {
				return new string[] { "origin", "direction" };
			}
		}

		public void OnRegisterPorts() {
			ray = RegisterInputValue<Ray>("ray");
		}

		public override Vector3 Get_0() {
			return ray.value.origin;
		}

		public override Vector3 Get_1() {
			return ray.value.direction;
		}
	}
}
