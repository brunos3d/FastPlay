#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Editor {
	public class ContextItem {

		public int selected_index;

		public bool is_button;

		public GUIContent content = GUIContent.none;

		public object data;

		public Act<object> act;

		public ContextItem root;

		public ContextItem parent;

		public Dictionary<string, ContextItem> items = new Dictionary<string, ContextItem>();

		public ContextItem() { }

		public ContextItem AddItem(GUIContent content, Act<object> act, object data) {
			this.root = this;
			this.parent = this;
			return AddItem(content, this, act, data);
		}

		public ContextItem AddItem(GUIContent content, ContextItem root, Act<object> act, object data) {
			//root/Events/Lifecycle/Start
			//root/Events/Lifecycle/Update
			List<string> paths = content.text.Split('/').Where(s => !s.IsNullOrWhiteSpace()).ToList();
			string root_path = paths[0];
			string end_path = paths[paths.Count - 1];
			//final pass "Start"
			if (root_path == end_path) {
				ContextItem instance = new ContextItem();
				instance.is_button = true;
				instance.content = new GUIContent(root_path, content.image, content.tooltip);
				instance.act = act;
				instance.data = data;
				instance.root = root;
				instance.parent = this;
				return items[root_path] = instance;
			}
			//other pass "Events" or "Mono"
			else {
				string subpath = content.text.Replace(string.Format("{0}/", root_path), string.Empty);
				ContextItem ci_1;
				if (items.TryGetValue(root_path, out ci_1)) {
					ci_1.is_button = false;
					return ci_1.AddItem(new GUIContent(subpath, content.image, content.tooltip), root, act, data);
				}
				//first pass "Events" or "Mono" of Start register
				else {
					ContextItem instance = new ContextItem();
					instance.content = new GUIContent(root_path, null, content.tooltip);
					instance.root = root;
					instance.parent = this;
					instance.AddItem(new GUIContent(subpath, content.image, content.tooltip), root, act, data);
					return items[root_path] = instance;
				}
			}
		}
	}
}
#endif
