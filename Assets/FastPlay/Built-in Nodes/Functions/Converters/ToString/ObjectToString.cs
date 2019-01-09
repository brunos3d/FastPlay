using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("ToString")]
	[Path("Functions/Converters/ToString/Object")]
	[Body("ToString", "Object")]
	public class ObjectToString : ValueNode<string>, IRegisterPorts {

		public InputValue<object> obj;

		public void OnRegisterPorts() {
			obj = RegisterInputValue<object>("object");
		}

		public override string OnGetValue() {
			return obj.value.ToString();
		}
	}
}
