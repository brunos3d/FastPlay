using System;
using UnityEngine;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(object), typeof(string))]
	public class SystemObjectToString : IValueConverter<string> {

		public SystemObjectToString() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(object).IsAssignableFrom(from) && typeof(string).IsAssignableFrom(to);
		}

		public string Convert(object value) {
			return value.ToString();
		}
	}
}
