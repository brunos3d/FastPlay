using System;
using UnityEngine;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(Vector3), typeof(Vector2))]
	public class Vector3ToVector2 : IValueConverter<Vector2> {

		public Vector3ToVector2() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(Vector3).IsAssignableFrom(from) && typeof(Vector2).IsAssignableFrom(to);
		}

		public Vector2 Convert(object value) {
			return (Vector2)value;
		}
	}
}
