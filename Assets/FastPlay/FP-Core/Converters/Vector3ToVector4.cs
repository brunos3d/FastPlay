using System;
using UnityEngine;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(Vector3), typeof(Vector4))]
	public class Vector3ToVector4 : IValueConverter<Vector4> {

		public Vector3ToVector4() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(Vector3).IsAssignableFrom(from) && typeof(Vector4).IsAssignableFrom(to);
		}

		public Vector4 Convert(object value) {
			return (Vector3)value;
		}
	}
}
