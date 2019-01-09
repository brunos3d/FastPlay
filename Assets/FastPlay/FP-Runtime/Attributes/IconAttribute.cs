using System;
using System.Collections.Generic;
using UnityEngine;
using FastPlay.Editor;

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.Class)]
	public class IconAttribute : Attribute {

		public string icon_path;

		private static Dictionary<string, Texture> icons = new Dictionary<string, Texture>();

		public IconAttribute(string icon_path) {
			this.icon_path = icon_path;
		}

		public Texture GetIcon() {
			Texture t;
			if (icons.TryGetValue(icon_path, out t)) {
				return t;
			}
			else {
#if UNITY_EDITOR
				return icons[icon_path] = EditorUtils.FindAssetByName<Texture>(icon_path);
#else
				return icons[icon_path] = Resources.Load<Texture>(icon_path);
#endif
			}
		}
	}
}
