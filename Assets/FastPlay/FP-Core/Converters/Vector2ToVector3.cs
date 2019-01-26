using System;
using UnityEngine;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(Vector2), typeof(Vector3))]
	public class Vector2ToVector3 : IValueConverter<Vector3> {

		public Vector2ToVector3() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(Vector2).IsAssignableFrom(from) && typeof(Vector3).IsAssignableFrom(to);
		}

		public Vector3 Convert(object value) {
			return (Vector2)value;
		}
	}
}
