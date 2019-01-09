using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Time.time")]
	[Path("Actions/Time/Time.time")]
	[Body("time", "Time", "clock_node_icon")]
	public class TimeTime : ValueNode<float> {

		public override float OnGetValue() {
			return Time.time;
		}
	}
}
