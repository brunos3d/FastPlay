namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("mouse_select_up")]
	[Title("OnMouseUpAsButton")]
	[Path("Events/Mouse/OnMouseUpAsButton")]
	[Summary("OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.")]
	public class OnMouseUpAsButtonEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseUpAsButton += OnMouseUpAsButton;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseUpAsButton -= OnMouseUpAsButton;
		}

		public void OnMouseUpAsButton() {
			Call(output);
		}
	}
}
