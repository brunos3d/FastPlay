using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Add")]
	[Path("Functions/Operation/Vector4/Add")]
	[Body("Add", "Vector4")]
	public class Vector4Add : ValueNode<Vector4>, IRegisterPorts {

		public InputValue<Vector4> a, b;

		protected override string getDefaultName { get { return "A + B"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<Vector4>("A");
			b = RegisterInputValue<Vector4>("B");
		}

		public override Vector4 OnGetValue() {
			return a.value + b.value;
		}
	}
}
