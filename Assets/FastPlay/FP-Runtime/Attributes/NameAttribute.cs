using System;

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.Class)]
	public class NameAttribute : Attribute {

		public string name;

		public NameAttribute(string name) {
			this.name = name;
		}
	}
}
