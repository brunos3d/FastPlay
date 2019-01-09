using FastPlay.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[AttributeUsage(AttributeTargets.Class)]
	public class BodyAttribute : Attribute {

		public string title = string.Empty;

		public string icon_path = string.Empty;

		public string description = string.Empty;

		public bool slim = false;

		private static Dictionary<string, Texture> icons = new Dictionary<string, Texture>();

		public BodyAttribute() { }

		public BodyAttribute(string title) {
			this.title = title;
		}

		public BodyAttribute(string title, string description) {
			this.title = title;
			this.description = description;
		}

		public BodyAttribute(string title, string description, string icon_path) {
			this.title = title;
			this.description = description;
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
