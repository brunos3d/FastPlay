namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("mouse_select_down")]
	[Title("OnMouseDown")]
	[Path("Events/Mouse/OnMouseDown")]
	[Summary("OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.")]
	public class OnMouseDownEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseDown += OnMouseDown;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseDown -= OnMouseDown;
		}

		public void OnMouseDown() {
			Call(output);
		}
	}
}
