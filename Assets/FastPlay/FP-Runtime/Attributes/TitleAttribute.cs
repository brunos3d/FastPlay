using FastPlay.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.Class)]
	public class TitleAttribute : Attribute {

		public string title = string.Empty;

		public TitleAttribute() { }

		public TitleAttribute(string title) {
			this.title = title;
		}
	}
}
