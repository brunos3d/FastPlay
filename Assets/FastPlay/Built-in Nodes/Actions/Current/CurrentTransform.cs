using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Slim]
	[Title("transform")]
	[Subtitle("Current")]
	[Path("Actions/Current/Transform")]
	public class CurrentTransform : ValueNode<Transform> {

		public override Transform OnGetValue() {
			return Current.transform;
		}
	}
}
