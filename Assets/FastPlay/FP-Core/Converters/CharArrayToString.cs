using System;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(char[]), typeof(string))]
	public class CharArrayToString : IValueConverter<string> {

		public CharArrayToString() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(char[]).IsAssignableFrom(from) && typeof(string).IsAssignableFrom(to);
		}

		public string Convert(object value) {
			return new string((char[])value);
		}
	}
}
