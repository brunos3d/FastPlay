using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("RaycastHit Info")]
	[Path("Functions/Extractors/RaycastHit")]
	[Body("Info", "RaycastHit")]
	public class RaycastHitInfo : MultiValueNode<Vector3, Transform, Collider, float>, IRegisterPorts {

		public InputValue<RaycastHit> hit;

		public override string[] portNames {
			get {
				return new string[] { "point", "transform", "collider", "distance" };
			}
		}

		public void OnRegisterPorts() {
			hit = RegisterInputValue<RaycastHit>("raycastHit");
		}

		public override Vector3 Get_0() {
			return hit.value.point;
		}

		public override Transform Get_1() {
			return hit.value.transform;
		}

		public override Collider Get_2() {
			return hit.value.collider;
		}

		public override float Get_3() {
			return hit.value.distance;
		}
	}
}
