using System;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(int), typeof(float))]
	public class IntToFloat : IValueConverter<float> {

		public IntToFloat() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(int).IsAssignableFrom(from) && typeof(float).IsAssignableFrom(to);
		}

		public float Convert(object value) {
			return (int)value;
		}
	}
}
