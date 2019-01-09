using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Bounds Info")]
	[Path("Functions/Extractors/Bounds")]
	[Body("Info", "Bounds")]
	public class BoundsInfo : MultiValueNode<Vector3, Vector3>, IRegisterPorts {

		public InputValue<Bounds> bounds;

		public override string[] portNames {
			get {
				return new string[] { "center", "size" };
			}
		}

		public void OnRegisterPorts() {
			bounds = RegisterInputValue<Bounds>("bounds");
		}

		public override Vector3 Get_0() {
			return bounds.value.center;
		}

		public override Vector3 Get_1() {
			return bounds.value.size;
		}
	}
}
