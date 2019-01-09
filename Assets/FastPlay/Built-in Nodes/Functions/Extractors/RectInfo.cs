using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Rect Info")]
	[Path("Functions/Extractors/Rect")]
	[Body("Info", "Rect")]
	public class RectInfo : MultiValueNode<float, float, float, float>, IRegisterPorts {

		public InputValue<Rect> _rect;

		public override string[] portNames {
			get {
				return new string[] { "X", "Y", "widht", "height" };
			}
		}

		public void OnRegisterPorts() {
			_rect = RegisterInputValue<Rect>("rect");
		}

		public override float Get_0() {
			return _rect.value.x;
		}

		public override float Get_1() {
			return _rect.value.y;
		}

		public override float Get_2() {
			return _rect.value.width;
		}

		public override float Get_3() {
			return _rect.value.height;
		}
	}
}
