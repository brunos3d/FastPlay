using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Vector2 Info")]
	[Path("Functions/Extractors/Vector2")]
	[Body("Info", "Vector2")]
	public class Vector2Info : MultiValueNode<float, float>, IRegisterPorts {

		public InputValue<Vector2> vector;

		public override string[] portNames {
			get {
				return new string[] { "X", "Y" };
			}
		}

		public void OnRegisterPorts() {
			vector = RegisterInputValue<Vector2>("vector2");
		}

		public override float Get_0() {
			return vector.value.x;
		}

		public override float Get_1() {
			return vector.value.y;
		}
	}
}
