using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Current.GetComponent")] 
	[Path("Actions/Current/GetComponent")]
	[Body("GetComponent", "Current", slim = true)]
	public class CurrentGetComponent<T> : ValueNode<T> where T : Component {

		public override T OnGetValue() {
			return Current.GetComponent<T>();
		}
	}
}
