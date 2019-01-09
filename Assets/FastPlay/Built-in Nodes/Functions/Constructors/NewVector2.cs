using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New Vector2")]
	[Path("Functions/Constructors/New Vector2")]
	[Body("New Vector2")]
	public class NewVector2 : ValueNode<Vector2>, IRegisterPorts {

		public InputValue<float> x, y;

		public void OnRegisterPorts() {
			Vector2 defaultVector2 = default(Vector2);
			x = RegisterInputValue<float>("X", defaultVector2.x);
			y = RegisterInputValue<float>("Y", defaultVector2.y);
		}

		public override Vector2 OnGetValue() {
			return new Vector2(x.value, y.value);
		}
	}
}
