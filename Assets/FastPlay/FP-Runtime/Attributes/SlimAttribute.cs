using FastPlay.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.Class)]
	public class SlimAttribute : Attribute {

		public bool is_slim = true;

		public SlimAttribute() { }

		public SlimAttribute(bool is_slim) {
			this.is_slim = is_slim;
		}
	}
}
