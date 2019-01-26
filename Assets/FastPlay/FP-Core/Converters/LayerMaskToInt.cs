using System;
using UnityEngine;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(LayerMask), typeof(int))]
	public class LayerMaskToInt : IValueConverter<int> {

		public LayerMaskToInt() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(LayerMask).IsAssignableFrom(from) && typeof(int).IsAssignableFrom(to);
		}

		public int Convert(object value) {
			return ((LayerMask)value).value;
		}
	}
}
