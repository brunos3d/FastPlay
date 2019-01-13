using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnCanvasGroupChanged")]
	[Path("Events/UI/OnCanvasGroupChanged")]
	public class OnCanvasGroupChangedEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoCanvasGroupChanged += OnCanvasGroupChanged;
		}

		public void OnRemoveEvents() {
			Current.controller.DoCanvasGroupChanged -= OnCanvasGroupChanged;
		}

		public void OnCanvasGroupChanged() {
			Call(output);
		}
	}
}
