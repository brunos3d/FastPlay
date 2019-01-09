#if UNITY_EDITOR
using System;
using UnityEngine;

namespace FastPlay.Runtime {
	// Editor Port 
	public abstract partial class Port {

		[NonSerialized]
		public float unit_delta_size;

		public Rect rect {
			get {
				return Node.GetPortPoint(this);
			}
		}

		public Color color {
			get {
				return Node.GetPortColor(this);
			}
		}
	}
}
#endif
