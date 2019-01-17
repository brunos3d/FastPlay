using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnRenderObject")]
	[Path("Events/Rendering/OnRenderObject")]
	[Summary("OnRenderObject is called after camera has rendered the Scene.")]
	public class OnRenderObjectEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoRenderObject += OnRenderObject;
		}

		public void OnRemoveEvents() {
			Current.controller.DoRenderObject -= OnRenderObject;
		}

		public void OnRenderObject() {
			Call(output);
		}
	}
}
