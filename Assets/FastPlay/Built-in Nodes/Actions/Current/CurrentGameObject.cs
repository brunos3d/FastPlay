using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Current.gameObject")]
	[Path("Actions/Current/GameObject")]
	[Body("GameObject", "Current", slim = true)]
	public class CurrentGameObject : ValueNode<GameObject> {

		public override GameObject OnGetValue() {
			return Current.gameObject;
		}
	}
}
