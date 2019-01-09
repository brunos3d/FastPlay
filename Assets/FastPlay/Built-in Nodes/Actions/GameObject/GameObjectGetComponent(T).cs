using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("GetComponent")]
	[Path("Actions/GameObject/GetComponent")]
	[Body("GetComponent", "GameObject")]
	public class GameObjectGetComponent<T> : ValueNode<T>, IRegisterPorts where T : Component {

		public InputValue<GameObject> target;

		public void OnRegisterPorts() {
			target = RegisterInputValue<GameObject>("target");
		}

		public override T OnGetValue() {
			return target.value.GetComponent<T>();
		}
	}
}
