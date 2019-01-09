using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Current.transform")]
	[Path("Actions/Current/Transform")]
	[Body("Transform", "Current", slim = true)]
	public class CurrentTransform : ValueNode<Transform> {

		public override Transform OnGetValue() {
			return Current.transform;
		}
	}
}
