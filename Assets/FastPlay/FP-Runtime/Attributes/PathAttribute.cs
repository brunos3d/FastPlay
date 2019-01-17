using System;

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.Class)]
	public class PathAttribute : Attribute {

		public string path;

		public PathAttribute(string path) {
			this.path = path;
		}
	}
}
