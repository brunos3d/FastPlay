namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("mouse_select_drag")]
	[Title("OnMouseDrag")]
	[Path("Events/Mouse/OnMouseDrag")]
	[Summary("OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.")]
	public class OnMouseDragEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseDrag += OnMouseDrag;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseDrag -= OnMouseDrag;
		}

		public void OnMouseDrag() {
			Call(output);
		}
	}
}
