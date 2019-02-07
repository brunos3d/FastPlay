namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("arrow_repeat")]
	[Title("Update")]
	[Path("Events/Lifecycle/Update")]
	[Summary("Update is called every frame, if the MonoBehaviour is enabled.")]
	public class UpdateEvent : EventNode, IUpdate {

		public void Update() {
			Call(output);
		}
	}
}
