using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Subtract")]
	[Subtitle("Vector2")]
	[Path("Functions/Operation/Vector2/Subtract")]
	public class Vector2Subtract : ValueNode<Vector2>, IRegisterPorts {

		public InputValue<Vector2> a, b;

		protected override string getDefaultName { get { return "A - B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<Vector2>("A");
			b = RegisterInputValue<Vector2>("B");
		}

		public override Vector2 OnGetValue() {
			return a.value - b.value;
		}
	}
}
