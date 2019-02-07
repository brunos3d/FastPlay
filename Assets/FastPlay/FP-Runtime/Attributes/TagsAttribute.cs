using System;

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.All)]
	public class TagsAttribute : Attribute {

		public string joinned_tags;

		public string[] tags;

		public TagsAttribute(params string[] tags) {
			this.joinned_tags = string.Join(", ", tags);
			this.tags = tags;
		}
	}
}
