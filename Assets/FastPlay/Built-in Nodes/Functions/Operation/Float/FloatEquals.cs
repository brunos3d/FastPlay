using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Equals")]
	[Subtitle("Float")]
	[Path("Functions/Operation/Float/Equals")]
	public class FloatEquals : ValueNode<bool>, IRegisterPorts {

		public InputValue<float> a, b;

		protected override string getDefaultName { get { return "A == B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<float>("A");
			b = RegisterInputValue<float>("B");
		}

		public override bool OnGetValue() {
			return a.value == b.value;
		}
	}
}
