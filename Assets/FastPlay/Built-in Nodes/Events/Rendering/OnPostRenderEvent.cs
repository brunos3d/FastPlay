using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnPostRender")]
	[Path("Events/Rendering/OnPostRender")]
	[Summary("OnPostRender is called after a camera finished rendering the Scene.")]
	public class OnPostRenderEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoPostRender += OnPostRender;
		}

		public void OnRemoveEvents() {
			Current.controller.DoPostRender -= OnPostRender;
		}

		public void OnPostRender() {
			Call(output);
		}
	}
}
