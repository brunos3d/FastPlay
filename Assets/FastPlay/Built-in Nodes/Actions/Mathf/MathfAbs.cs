using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Mathf.Abs")]
	[Path("Actions/Mathf/Mathf.Abs")]
	[Body("Abs", "Mathf")]
	public class MathfAbs : ValueNode<float>, IRegisterPorts {

		public InputValue<float> f;

		public void OnRegisterPorts() {
			f = RegisterInputValue<float>("f");
		}

		public override float OnGetValue() {
			return Mathf.Abs(f.value);
		}
	}
}
