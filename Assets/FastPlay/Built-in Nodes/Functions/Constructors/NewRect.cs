using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New Rect")]
	[Path("Functions/Constructors/New Rect")]
	[Body("New Rect")]
	public class NewRect : ValueNode<Rect>, IRegisterPorts {

		public InputValue<float> x, y, width, height;

		public void OnRegisterPorts() {
			Rect defaultRect = default(Rect);
			x = RegisterInputValue<float>("X", defaultRect.x);
			y = RegisterInputValue<float>("Y", defaultRect.y);
			width = RegisterInputValue<float>("width", defaultRect.width);
			height = RegisterInputValue<float>("height", defaultRect.height);
		}

		public override Rect OnGetValue() {
			return new Rect(x.value, y.value, width.value, height.value);
		}
	}
}
