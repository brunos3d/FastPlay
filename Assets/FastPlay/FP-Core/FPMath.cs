using UnityEngine;
using System.Collections.Generic;

namespace FastPlay {
	public static class FPMath {

		public static float SnapValue(float value, float snap = 20.0f) {
			return Mathf.Round(value / snap) * snap;
		}

		public static Vector2 SnapVector2(Vector2 value, float snap = 20.0f) {
			return new Vector2(SnapValue(value.x, snap), SnapValue(value.y, snap));
		}

		public static Vector2 CenterOfPoints(List<Vector2> points) {
			return CenterOfPoints(points.ToArray());
		}

		public static Vector2 CenterOfPoints(params Vector2[] points) {
			float count = points.Length;
			Vector2 center = Vector2.zero;

			foreach (Vector2 point in points) {
				center += point;
			}
			center /= count;

			return center;
		}
	}
}
