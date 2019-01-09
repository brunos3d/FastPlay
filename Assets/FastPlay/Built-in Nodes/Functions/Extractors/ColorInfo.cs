using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Color Info")]
	[Path("Functions/Extractors/Color")]
	[Body("Info", "Color")]
	public class ColorInfo : MultiValueNode<float, float, float, float>, IRegisterPorts {

		public InputValue<Color> color;

		public override string[] portNames {
			get {
				return new string[] { "R", "G", "B", "A" };
			}
		}

		public void OnRegisterPorts() {
			color = RegisterInputValue<Color>("color");
		}

		public override float Get_0() {
			return color.value.r;
		}

		public override float Get_1() {
			return color.value.g;
		}

		public override float Get_2() {
			return color.value.b;
		}

		public override float Get_3() {
			return color.value.a;
		}
	}
}
