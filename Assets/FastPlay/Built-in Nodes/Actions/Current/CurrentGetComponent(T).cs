using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("GetComponent")] 
	[Subtitle("Current")]
	[Path("Actions/Current/GetComponent<T>")]
	public class CurrentGetComponent<T> : ValueNode<T> where T : Component {

		public CurrentGetComponent() { }

		public override T OnGetValue() {
			return Current.GetComponent<T>();
		}
	}
}
