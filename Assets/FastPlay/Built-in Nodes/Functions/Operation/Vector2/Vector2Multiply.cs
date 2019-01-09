using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Multiply")]
	[Path("Functions/Operation/Vector2/Multiply")]
	[Body("Multiply", "Vector2")]
	public class Vector2Multiply : ValueNode<Vector2>, IRegisterPorts {

		public InputValue<Vector2> vector;

		public InputValue<float> factor;

		protected override string getDefaultName { get { return "Vector2 * Factor"; } }

		public void OnRegisterPorts() {
			vector = RegisterInputValue<Vector2>("Vector2");
			factor = RegisterInputValue<float>("Factor");
		}

		public override Vector2 OnGetValue() {
			return vector.value * factor.value;
		}
	}
}
