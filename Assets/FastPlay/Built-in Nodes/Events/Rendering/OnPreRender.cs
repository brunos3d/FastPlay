using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnPreRender")]
	[Path("Events/Rendering/OnPreRender")]
	public class OnPreRenderEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoPreRender += OnPreRender;
		}

		public void OnRemoveEvents() {
			Current.controller.DoPreRender -= OnPreRender;
		}

		public void OnPreRender() {
			Call(output);
		}
	}
}
