namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("tick")]
	[Title("OnEnable")]
	[Path("Events/Lifecycle/OnEnable")]
	[Summary("This function is called when the object becomes enabled and active.")]
	public class OnEnableEvent : EventNode {

		public void OnEnable() {
			Call(output);
		}
	}
}
