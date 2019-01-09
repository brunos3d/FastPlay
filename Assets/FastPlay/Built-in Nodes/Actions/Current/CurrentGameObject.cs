using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("gameObject")]
	[Subtitle("Current")]
	[Path("Actions/Current/GameObject")]
	public class CurrentGameObject : ValueNode<GameObject> {

		public override GameObject OnGetValue() {
			return Current.gameObject;
		}
	}
}
