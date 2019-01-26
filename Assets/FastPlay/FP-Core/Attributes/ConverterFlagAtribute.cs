using System;

namespace FastPlay {
	[AttributeUsage(AttributeTargets.Class)]
	public class ConverterFlagAtribute : Attribute {

		public Type base_type;

		public Type from;

		public Type to;

		public bool CanConvert(Type from, Type to) {
			return this.from.IsAssignableFrom(from) && this.to.IsAssignableFrom(to);
		}

		public ConverterFlagAtribute(Type from, Type to) {
			this.from = from;
			this.to = to;
		}
	}
}
