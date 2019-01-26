using System;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(string), typeof(float))]
	public class StringToFloat : IValueConverter<float> {

		public StringToFloat() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(string).IsAssignableFrom(from) && typeof(float).IsAssignableFrom(to);
		}

		public float Convert(object value) {
			return float.Parse((string)value);
		}
	}
}
