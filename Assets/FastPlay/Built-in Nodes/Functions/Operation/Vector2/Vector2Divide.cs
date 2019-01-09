using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Divide")]
	[Path("Functions/Operation/Vector2/Divide")]
	[Body("Divide", "Vector2")]
	public class Vector2Divide : ValueNode<Vector2>, IRegisterPorts {

		public InputValue<Vector2> vector;

		public InputValue<float> factor;

		protected override string getDefaultName { get { return "Vector2 / Factor"; } }

		public void OnRegisterPorts() {
			vector = RegisterInputValue<Vector2>("Vector2");
			factor = RegisterInputValue<float>("Factor");
		}

		public override Vector2 OnGetValue() {
			return vector.value / factor.value;
		}
	}
}
