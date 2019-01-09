using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Divide")]
	[Subtitle("Vector3")]
	[Path("Functions/Operation/Vector3/Divide")]
	public class Vector3Divide : ValueNode<Vector3>, IRegisterPorts {

		public InputValue<Vector3> vector;

		public InputValue<float> factor;

		protected override string getDefaultName { get { return "Vector3 / Factor"; } }

		public void OnRegisterPorts() {
			vector = RegisterInputValue<Vector3>("Vector3");
			factor = RegisterInputValue<float>("Factor");
		}

		public override Vector3 OnGetValue() {
			return vector.value / factor.value;
		}
	}
}
