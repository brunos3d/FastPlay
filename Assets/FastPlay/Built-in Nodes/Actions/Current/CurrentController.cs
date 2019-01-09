using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Current.controller")]
	[Body("Controller", "Current")]
	[Path("Actions/Current/Controller")]
	public class CurrentController : ValueNode<GraphController> {

		public override GraphController OnGetValue() {
			return Current.controller;
		}
	}
}
