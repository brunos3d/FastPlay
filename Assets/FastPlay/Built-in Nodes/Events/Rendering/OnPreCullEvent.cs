using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnPreCull")]
	[Path("Events/Rendering/OnPreCull")]
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
