using FastPlay.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.Class)]
	public class SubtitleAttribute : Attribute {

		public string subtitle = string.Empty;

		public SubtitleAttribute() { }

		public SubtitleAttribute(string subtitle) {
			this.subtitle = subtitle;
		}
	}
}
