using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnPreCull")]
	[Path("Events/Rendering/OnPreCull")]
	[Summary("OnPreCull is called before a camera culls the Scene.")]
	public class OnPreCullEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoPreCull += OnPreCull;
		}

		public void OnRemoveEvents() {
			Current.controller.DoPreCull -= OnPreCull;
		}

		public void OnPreCull() {
			Call(output);
		}
	}
}
