using System;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(float), typeof(int))]
	public class FloatToInt : IValueConverter<int> {

		public FloatToInt() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(float).IsAssignableFrom(from) && typeof(int).IsAssignableFrom(to);
		}

		public int Convert(object value) {
			return (int)(float)value;
		}
	}
}
