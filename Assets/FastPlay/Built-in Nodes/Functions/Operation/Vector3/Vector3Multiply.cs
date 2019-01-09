using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Multiply")]
	[Subtitle("Vector3")]
	[Path("Functions/Operation/Vector3/Multiply")]
	public class Vector3Multiply : ValueNode<Vector3>, IRegisterPorts {

		public InputValue<Vector3> vector;

		public InputValue<float> factor;

		protected override string getDefaultName { get { return "Vector3 * Factor"; } }

		public void OnRegisterPorts() {
			vector = RegisterInputValue<Vector3>("Vector3");
			factor = RegisterInputValue<float>("Factor");
		}

		public override Vector3 OnGetValue() {
			return vector.value * factor.value;
		}
	}
}
