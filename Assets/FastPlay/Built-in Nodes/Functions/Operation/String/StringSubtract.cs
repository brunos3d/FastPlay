using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Subtract")]
	[Path("Functions/Operation/String/Subtract")]
	[Body("Subtract", "String")]
	public class StringSubtract : ValueNode<string>, IRegisterPorts {

		public InputValue<string> a, b;

		protected override string getDefaultName { get { return "A.Replace(B, empty)"; } }

		public void OnRegisterPorts() {
			a = RegisterInputValue<string>("A");
			b = RegisterInputValue<string>("B");
		}

		public override string OnGetValue() {
			return a.value.Replace(b.value, string.Empty);
		}
	}
}
