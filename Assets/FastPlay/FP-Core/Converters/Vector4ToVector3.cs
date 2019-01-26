using System;
using UnityEngine;

namespace FastPlay {
	[ConverterFlagAtribute(typeof(Vector4), typeof(Vector3))]
	public class Vector4ToVector3 : IValueConverter<Vector3> {

		public Vector4ToVector3() { }

		public bool CanConvert(Type from, Type to) {
			return typeof(Vector4).IsAssignableFrom(from) && typeof(Vector3).IsAssignableFrom(to);
		}

		public Vector3 Convert(object value) {
			return (Vector4)value;
		}
	}
}
