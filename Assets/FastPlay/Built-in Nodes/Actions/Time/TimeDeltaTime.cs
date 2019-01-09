using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Time.deltaTime")]
	[Path("Actions/Time/Time.deltaTime")]
	[Body("deltaTime", "Time", "clock_node_icon")]
	public class TimeDeltaTime : ValueNode<float> {

		public override float OnGetValue() {
			return Time.deltaTime;
		}
	}
}
