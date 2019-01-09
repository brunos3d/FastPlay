using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Controller")]
	[Subtitle("Current")]
	[Path("Actions/Current/Controller")]
	public class CurrentController : ValueNode<GraphController> {

		public override GraphController OnGetValue() {
			return Current.controller;
		}
	}
}
