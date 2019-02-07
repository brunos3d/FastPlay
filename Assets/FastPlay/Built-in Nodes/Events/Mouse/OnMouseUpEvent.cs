namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("mouse_select_up")]
	[Title("OnMouseUp")]
	[Path("Events/Mouse/OnMouseUp")]
	[Summary("OnMouseUp is called when the user has released the mouse button.")]
	public class OnMouseUpEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseUp += OnMouseUp;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseUp -= OnMouseUp;
		}

		public void OnMouseUp() {
			Call(output);
		}
	}
}
