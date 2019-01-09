using FastPlay.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.Class)]
	public class PathAttribute : Attribute {

		public string path;

		public PathAttribute(string path) {
			this.path = path;
		}
	}
}
