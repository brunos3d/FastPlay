using System;

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.All)]
	public class SummaryAttribute : Attribute {

		public string summary;

		public string[] parameters;

		public SummaryAttribute(string summary, params string[] parameters) {
			this.summary = summary;
			this.parameters = parameters;
		}
	}
}
